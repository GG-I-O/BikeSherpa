import { ServicesIndentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { HateoasLinks, Link } from "@/models/HateoasLink";
// import { ResourceNotification, ResourceOperation } from "@/infra/storage/notification/Notification";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService, IStorageContext } from "@/spi/StorageSPI";
import { Observable, observable } from "@legendapp/state";
import { ObservablePersistMMKV } from "@legendapp/state/persist-plugins/mmkv";
import { syncedCrud } from "@legendapp/state/sync-plugins/crud";
import * as Network from 'expo-network';
import { inject } from "inversify";

export default abstract class AbstractStorageContext<T extends { id: string }> implements IStorageContext<T> {
    protected logger: any;
    protected store: Observable<Record<T extends {
        id: number;
    } ? number : string, T>>;
    private initialLoad: boolean = true;
    private canSync$: Observable<boolean>;

    // protected notificationService: INotificationService;
    private resourceName: string;

    protected constructor(
        storeName: string,
        // @inject(ServicesIndentifiers.Logger) logger: ILogger,
        // @inject(ServicesIndentifiers.NotificationService) notificationService: INotificationService
    ) {
        // this.logger = logger.extend(storeName);
        this.store = this.initStore(storeName);

        this.canSync$ = observable(false);

        this.resourceName = storeName.toLocaleLowerCase();
        // this.notificationService = notificationService;

        // Init canSync observable + connect to notification service if canSync
        this.initNetworkState();
    }

    private async initNetworkState() {
        const networkState = await Network.getNetworkStateAsync();
        if (networkState.isInternetReachable) {
            // if (this.notificationService) {
            //     try {
            //         await this.notificationService.start(this.resourceName);
            //         this.logger.info('NotificationService started, enabling sync');
            //     } catch (error) {
            //         this.logger.error('Failed to start NotificationService', error);
            //     }
            // }
            this.canSync$.set(true);
            this.initialLoad = false;
        }

        // Subscribe to network state changes
        Network.addNetworkStateListener(async ({ isInternetReachable }) => {
            this.logger.debug("Is Internet Reachable?", isInternetReachable);
            console.debug("Internet console :", isInternetReachable);
            if (isInternetReachable !== undefined) {
                this.canSync$.set(isInternetReachable);
            }
        });
    }

    public getStore() {
        return this.store;
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
                const result = await this.create(item);
                return result; // return server version
            },
            update: async (item: T) => {
                // Check if we have the rights to update
                // if (!item.links?.some((link: Link) => link.rel == "update")) return;

                const result = await this.update(item);
                return result; // return server version
            },
            delete: async (item: T) => {
                // Check if we have the rights to delete
                // if (!item.links?.some((link: Link) => link.rel == "delete")) return;

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

            // Retry configuration - May be useless with waitFor and WaitForSet
            retry: {
                infinite: true, // Keep retrying forever
                backoff: 'exponential',
                delay: 10000, // Start with 10 seconds delay
                maxDelay: 60000 // Max 60 seconds between retries
            },

            // Wait for authentication before syncing
            waitFor: () => this.canSync$,
            waitForSet: () => this.canSync$,

            // Subscribe to NotificationService
            // subscribe: ({ lastSync, refresh, update }) => {
            //     this.logger.info(`Setting up NotificationService subscription for ${storeName}`);

            //     const connection = this.notificationService.getConnection();

            //     if (!connection) {
            //         this.logger.warn('NotificationService connection not available yet');
            //         return () => { };
            //     }

            //                     const onNotification = async (notification: ResourceNotification) => {
            //                         switch (notification.operation) {
            //                             case ResourceOperation.POST:
            //                                 this.logger.debug(`${this.resourceName} created via NotificationService`, notification.id);
            //                                 const postItem = await this.getItem(notification.id);
            //                                 if (postItem)
            //                                     update({ value: [postItem] });
            //                                 break;
            //                             case ResourceOperation.PUT:
            //                                 this.logger.debug(`${this.resourceName} updated via NotificationService`, notification.id);
            //                                 const putItem = await this.getItem(notification.id);
            //                                 if (putItem)
            //                                     update({ value: [putItem] });
            //                                 break;
            //                             case ResourceOperation.DELETE:
            //                                 this.logger.debug(`${this.resourceName} deleted via NotificationService`, notification.id);
            //                                 // Filter the deleted item out of the local data
            //                                 const localRecord: Record<string, T> = this.store.peek();
            //                                 let localArray = Object.values(localRecord);
            //                                 localArray = localArray.filter((item: T) => item.id != notification.id);
            //                                 // Use mode: 'set' to replace instead of merge
            //                                 update({ value: localArray, mode: 'set' })
            //                                 break;
            //                             default:
            //                                 this.logger.error("Received a notification with unknown operation :", notification)
            //                                 throw new Error("Subscribe error");
            //                         }
            //                     }


            //     // Register event handlers
            //     connection.on("notification", onNotification);

            //     // Refresh on reconnection to catch any missed updates
            //     this.notificationService.onReconnected(async () => {
            //         this.logger.info(`NotificationService reconnected, refreshing ${storeName}`);
            //         const updatedStore = await this.getList();
            //         update({ value: updatedStore, mode: 'set' });
            //     });

            //     // Return cleanup function
            //     return () => {
            //         this.logger.info(`Cleaning up NotificationService subscription for ${storeName}`);
            //         // connection.off("notification", onNotification);
            //     };
            // },

            // Error handling
            onError: (error, params) => {
                this.logger.error("Customer sync error :", error, params);
            }
        }));
    }

    protected abstract getList(lastSync?: string): Promise<Array<T>>;
    protected abstract getItem(id: string): Promise<T | null>;
    protected abstract create(item: T): Promise<T>;
    protected abstract update(item: T): Promise<T>;
    protected abstract delete(item: T): Promise<void>;
}