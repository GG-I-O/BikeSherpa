import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService, IStorageContext } from "@/spi/StorageSPI";
import { Observable, observable } from "@legendapp/state";
import { ObservablePersistMMKV } from "@legendapp/state/persist-plugins/mmkv";
import { syncedCrud } from "@legendapp/state/sync-plugins/crud";
import * as Network from 'expo-network';
import { inject } from "inversify";
import { ResourceNotification, ResourceOperation } from "../notification/Notification";
import { HateoasLinks, hateoasRel, Link } from "@/models/HateoasLink";
import Storable from "@/models/Storable";
import ServerError from "@/models/ServerError";
import { EventRegister } from 'react-native-event-listeners';


export default abstract class AbstractStorageContext<T extends { id: string } & HateoasLinks & Storable> implements IStorageContext<T> {
    protected logger: ILogger;
    protected store: Observable<Record<T extends {
        id: number;
    } ? number : string, T>>;
    private initialLoad: boolean = true;
    private readonly canSync$: Observable<boolean>;

    private readonly notificationService: INotificationService;
    private readonly resourceName: string;

    private readonly onErrorEventType: string;

    protected constructor(
        storeName: string,
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService
    ) {
        this.logger = logger.extend(storeName);
        this.store = this.initStore(storeName);

        this.canSync$ = observable(false);

        this.resourceName = storeName.toLocaleLowerCase();
        this.notificationService = notificationService;
        this.onErrorEventType = `${this.resourceName}StorageError`;

        // Init canSync observable + connect to notification service if canSync
        this.initNetworkState().catch((error) => {
            this.logger.error('Failed to initialize network state', error);
        });
    }

    private async initNetworkState() {
        const networkState = await Network.getNetworkStateAsync();
        if (networkState.isInternetReachable) {
            if (this.notificationService) {
                try {
                    await this.notificationService.start(this.resourceName);
                    this.logger.info('NotificationService started, enabling sync');
                } catch (error) {
                    this.logger.error('Failed to start NotificationService', error);
                }
            }
            this.canSync$.set(true);
            this.initialLoad = false;
        }

        // Subscribe to network state changes
        Network.addNetworkStateListener(async ({ isInternetReachable }) => {
            this.logger.debug("Is Internet Reachable?", isInternetReachable);
            if (isInternetReachable !== undefined) {
                this.canSync$.set(isInternetReachable);
            }
        });
    }

    public getStore() {
        return this.store;
    }

    public subscribeToOnErrorEvent(callback: (error: string) => void): string {
        const eventId = EventRegister.addEventListener(this.onErrorEventType, callback);
        if (eventId) {
            return eventId as string;
        }
        return "";
    }

    public unsubscribeFromOnErrorEvent(id: string): void {
        EventRegister.removeEventListener(id);
    }

    protected getLinkHref(id: string, rel: string): string | undefined {
        const item = this.store.peek()[id];
        if (!item) return undefined;

        const link = item.links?.find((link: Link) => link.rel === rel);
        return link?.href;
    }

    private initStore(storeName: string) {
        return observable(syncedCrud<T>({
            list: async (listParams) => {
                // Initial load = get all to be in sync for sure
                if (this.initialLoad) {
                    this.initialLoad = false;
                    return await this.getList();
                }
                // Other loads = only get new changes
                const lastSync = listParams?.lastSync;
                const lastSyncDate: string | undefined = lastSync ? new Date(lastSync).toISOString() : undefined;
                return await this.getList(lastSyncDate);
            },
            create: async (item: T) => {
                item.createdAt = new Date().toISOString();
                item.updatedAt = new Date().toISOString();

                const result = await this.create(item);
                return await this.getItem(result);
            },
            update: async (item: T) => {
                // Check if we have the rights to update
                if (!item.links?.some((link: Link) => link.rel === hateoasRel.update)) {
                    return;
                }

                item.updatedAt = new Date().toISOString();

                await this.update(item);
                const result = await this.getItem(item.id);
                return result;
            },
            delete: async (item: T) => {
                // Check if we have the rights to delete
                if (!item.links?.some((link: Link) => link.rel === hateoasRel.delete)) return;

                await this.delete(item);
            },

            // Enable offline-first sync
            fieldId: 'id',
            fieldCreatedAt: 'createdAt',
            fieldUpdatedAt: 'updatedAt',

            // On first load, load all data, then only changes
            changesSince: this.initialLoad ? 'all' : 'last-sync',

            // Send whole data
            updatePartial: false,

            // Local persistence configuration
            persist: {
                plugin: ObservablePersistMMKV,
                name: storeName,
                retrySync: true // Persist pending changes and retry after restart
            },

            // Prevent undefined error
            initial: {} as Record<string, T>,

            // Automatically sync changes
            syncMode: 'auto',

            // Generate IDs for new customers
            generateId: () => crypto.randomUUID(),

            // Merge mode on incoming changes
            mode: 'merge',

            // Wait for authentication before syncing
            waitFor: () => this.canSync$,
            waitForSet: () => this.canSync$,

            // Subscribe to NotificationService
            subscribe: ({ update, refresh }) => {
                this.logger.info(`Setting up NotificationService subscription for ${storeName}`);

                const connection = this.notificationService.getConnection();

                if (!connection) {
                    this.logger.warn('NotificationService connection not available yet');
                    return () => { };
                }

                const onNotification = async (notification: ResourceNotification) => {
                    let localRecord: Record<string, T>;
                    switch (notification.operation) {
                        case ResourceOperation.POST:
                            this.logger.debug(`${this.resourceName} created via NotificationService`, notification.id);

                            if (notification.operationId) {
                                localRecord = this.store.peek();
                                let localArray = Object.values(localRecord);
                                const createdItem = localArray.find((item: T) => item.operationId === notification.operationId);
                                if (createdItem) {
                                    const backendItem = await this.getItem(notification.id);
                                    if (backendItem) {
                                        const index = localArray.findIndex((item: T) => item.id === createdItem.id);
                                        if (index !== -1) {
                                            localArray.splice(index, 1);
                                        }
                                        localArray.push(backendItem);
                                        update({ value: localArray, mode: 'set' });
                                    }
                                    break;
                                }
                            }

                            const postItem = await this.getItem(notification.id);
                            if (postItem) {
                                update({ value: [postItem] });
                            }
                            break;
                        case ResourceOperation.PUT:
                            this.logger.debug(`${this.resourceName} updated via NotificationService`, notification.id);
                            refresh();
                            break;
                        case ResourceOperation.DELETE:
                            this.logger.debug(`${this.resourceName} deleted via NotificationService`, notification.id);

                            // Filter the deleted item out of the local data
                            localRecord = this.store.peek();
                            let localArray = Object.values(localRecord);
                            localArray = localArray.filter((item: T) => item.id !== notification.id);
                            update({ value: localArray, mode: 'set' })// Use mode: 'set' to replace instead of merge

                            break;
                        default:
                            this.logger.error("Received a notification with unknown operation :", notification)
                            throw new Error("Subscribe error");
                    }
                }

                // Register event handlers
                connection.on("notification", onNotification);

                // Refresh on reconnection to catch any missed updates
                this.notificationService.onReconnected(async () => {
                    this.logger.info(`NotificationService reconnected, refreshing ${storeName}`);
                    const updatedStore = await this.getList();
                    update({ value: updatedStore, mode: 'set' });
                });

                // Return cleanup function
                return () => {
                    this.logger.info(`Cleaning up NotificationService subscription for ${storeName}`);
                    // connection.off("notification", onNotification);
                };
            },

            // Error handling
            onError: (error, params) => {
                this.logger.error(`${this.resourceName} storage error :`, error, params);

                if (this.canSync$ && params.revert) {
                    params.revert();
                }
                let serverErrors: ServerError[];
                try {
                    serverErrors = (error as any).response.data;
                    serverErrors.forEach((error) => EventRegister.emit(this.onErrorEventType, error.message));
                } catch (_error) {
                    EventRegister.emit(this.onErrorEventType, "Erreur du serveur");
                }
            }
        }));
    }

    protected abstract getList(lastSync?: string): Promise<T[]>;
    protected abstract getItem(id: string): Promise<T | null>;
    protected abstract create(item: T): Promise<string>;
    protected abstract update(item: T): Promise<void>;
    protected abstract delete(item: T): Promise<void>;
}