import { Observable } from "@legendapp/state";
import { HubConnection } from "@microsoft/signalr";

export interface IStorageContext<T> {
    getStore(): Observable<Record<T extends { id: number } ? number : string, T>>
}

export interface INotificationService {
    start(dataType: string): Promise<void>;
    stop(): Promise<void>;
    getConnection(): HubConnection | null;
    isConnected(): boolean;
    subscribeToDataType(dataType: string): Promise<void>;
    unsubscribeFromDataType(dataType: string): Promise<void>;
    onReconnected(callback: () => Promise<void>): void;
}