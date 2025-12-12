import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import { INotificationService } from '@/spi/StorageSPI';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import axios from 'axios';
import { inject, injectable } from 'inversify';

@injectable()
export class NotificationService implements INotificationService {
    private connection: HubConnection | null = null;
    private logger: ILogger;
    private reconnectCallbacks: (() => Promise<void>)[] = [];
    private subscribedDataTypes: string[] = [];

    constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.logger = logger.extend('NotificationService');
    }

    async start(dataType: string): Promise<void> {
        if (this.connection?.state === HubConnectionState.Connected) {
            this.logger.debug('Hub already connected');
            return;
        }

        try {
            this.connection = new HubConnectionBuilder()
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

            // Handle reconnection
            this.connection.onreconnected(async () => {
                this.logger.info('Hub reconnected');

                for (const dt of this.subscribedDataTypes) {
                    try {
                        await this.connection!.invoke('SubscribeToDataType', dt);
                    } catch (error) {
                        this.logger.error(`Error re-subscribing to data type ${dt}`, error);
                    }
                }

                this.reconnectCallbacks.forEach(async callback => await callback());
            });

            this.connection.onreconnecting(() => {
                this.logger.warn('Hub reconnecting...');
            });

            this.connection.onclose((error) => {
                this.logger.error('Hub connection closed', error);
            });

            await this.connection.start();
            this.logger.info('Hub connected successfully');

            await this.subscribeToDataType(dataType);
        } catch (error) {
            this.logger.error('Error starting Hub connection', error);
            throw error;
        }
    }

    async stop(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            this.logger.info('Hub connection stopped');
        }
    }

    /**
    * Subscribe to notifications for a specific data type
    */
    public async subscribeToDataType(dataType: string): Promise<void> {
        if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
            this.logger.warn(`Cannot subscribe to ${dataType}: not connected`);
            return;
        }

        try {
            await this.connection.invoke('SubscribeToDataType', dataType);
            this.subscribedDataTypes.push(dataType);
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
        if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
            this.logger.warn(`Cannot unsubscribe from ${dataType}: not connected`);
            return;
        }

        try {
            await this.connection.invoke('UnsubscribeFromDataType', dataType);
            this.subscribedDataTypes = this.subscribedDataTypes.filter(type => type !== dataType);
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

    onReconnected(callback: () => Promise<void>): void {
        this.reconnectCallbacks.push(callback);
    }
}