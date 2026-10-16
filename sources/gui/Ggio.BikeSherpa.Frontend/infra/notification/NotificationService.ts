import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import { INotificationService } from '@/spi/StorageSPI';
import { ResourceNotification } from '@/infra/notification/Notification';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import axios from 'axios';
import { inject, injectable } from 'inversify';

@injectable()
export class NotificationService implements INotificationService {
    private connection: HubConnection | null = null;
    private connecting: Promise<void> | null = null;
    private logger: ILogger;
    private reconnectCallbacks = new Set<() => Promise<void>>();
    private notificationHandlers = new Set<(notification: ResourceNotification) => void>();
    private subscribedDataTypes = new Set<string>();

    constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.logger = logger.extend('NotificationService');
    }

    /**
     * Ensures the hub is connected (one single connection, even if called concurrently)
     * and subscribes to the given data type.
     */
    async start(dataType: string): Promise<void> {
        await this.ensureConnected();
        await this.subscribeToDataType(dataType);
    }

    async stop(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            this.logger.info('Hub connection stopped');
        }
    }

    /**
     * Subscribe to notifications for a specific data type.
     * If the hub is not connected yet (or is reconnecting), the subscription is
     * remembered and applied as soon as the connection is (re)established.
     */
    public async subscribeToDataType(dataType: string): Promise<void> {
        this.subscribedDataTypes.add(dataType);

        if (!this.isConnected()) {
            this.logger.debug(`Not connected: ${dataType} will be subscribed once connected`);
            return;
        }

        try {
            await this.connection!.invoke('SubscribeToDataType', dataType);
            this.logger.info(`Subscribed to data type: ${dataType}`);
        } catch (error) {
            this.logger.error(`Error subscribing to data type ${dataType}`, error);
            throw error;
        }
    }

    /**
     * Unsubscribe from notifications for a specific data type
     */
    public async unsubscribeFromDataType(dataType: string): Promise<void> {
        this.subscribedDataTypes.delete(dataType);

        if (!this.isConnected()) {
            this.logger.debug(`Not connected: nothing to unsubscribe for ${dataType}`);
            return;
        }

        try {
            await this.connection!.invoke('UnsubscribeFromDataType', dataType);
            this.logger.info(`Unsubscribed from data type: ${dataType}`);
        } catch (error) {
            this.logger.error(`Error unsubscribing from data type ${dataType}`, error);
        }
    }

    getConnection(): HubConnection | null {
        return this.connection;
    }

    isConnected(): boolean {
        return this.connection?.state === HubConnectionState.Connected;
    }

    /**
     * Listen to every notification, independently of the connection lifecycle.
     * Returns an unsubscribe function.
     */
    onNotification(handler: (notification: ResourceNotification) => void): () => void {
        this.notificationHandlers.add(handler);
        return () => {
            this.notificationHandlers.delete(handler);
        };
    }

    /**
     * Called after the hub automatically reconnected. Returns an unsubscribe function.
     */
    onReconnected(callback: () => Promise<void>): () => void {
        this.reconnectCallbacks.add(callback);
        return () => {
            this.reconnectCallbacks.delete(callback);
        };
    }

    private ensureConnected(): Promise<void> {
        const state = this.connection?.state;

        // Reconnecting: SignalR handles it, subscriptions are re-applied in onreconnected
        if (state === HubConnectionState.Connected || state === HubConnectionState.Reconnecting) {
            return Promise.resolve();
        }

        if (!this.connecting) {
            this.connecting = this.connect().finally(() => {
                this.connecting = null;
            });
        }
        return this.connecting;
    }

    private async connect(): Promise<void> {
        // The HubConnection object is created once and reused: handlers registered on it
        // (by the storage contexts or by us) are never lost when a start() fails and is retried.
        if (!this.connection) {
            this.connection = this.buildConnection();
        }

        try {
            await this.connection.start();
        } catch (error) {
            this.logger.error('Error starting Hub connection', error);
            throw error;
        }

        this.logger.info('Hub connected successfully');
        await this.resubscribeAll();
    }

    private buildConnection(): HubConnection {
        const connection = new HubConnectionBuilder()
            .withUrl(`${axios.defaults.baseURL}/hubs/notifications`)
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: (retryContext) => {
                    // Exponential backoff: 0s, 2s, 10s, 30s, then 60s
                    if (retryContext.previousRetryCount === 0) return 0;
                    if (retryContext.previousRetryCount === 1) return 2000;
                    if (retryContext.previousRetryCount === 2) return 10000;
                    if (retryContext.previousRetryCount === 3) return 30000;
                    return 60000;
                }
            })
            .configureLogging(LogLevel.Information)
            .build();

        // Single dispatcher for onNotification() consumers
        connection.on('notification', (notification: ResourceNotification) => {
            this.notificationHandlers.forEach((handler) => {
                try {
                    handler(notification);
                } catch (error) {
                    this.logger.error('Notification handler failed', error);
                }
            });
        });

        connection.onreconnected(async () => {
            this.logger.info('Hub reconnected');
            await this.resubscribeAll();

            await Promise.all(
                Array.from(this.reconnectCallbacks).map((callback) =>
                    callback().catch((error) => this.logger.error('Reconnect callback failed', error))
                )
            );
        });

        connection.onreconnecting(() => {
            this.logger.warn('Hub reconnecting...');
        });

        connection.onclose((error) => {
            this.logger.error('Hub connection closed', error);
        });

        return connection;
    }

    private async resubscribeAll(): Promise<void> {
        for (const dataType of Array.from(this.subscribedDataTypes)) {
            try {
                await this.connection!.invoke('SubscribeToDataType', dataType);
            } catch (error) {
                this.logger.error(`Error re-subscribing to data type ${dataType}`, error);
            }
        }
    }
}