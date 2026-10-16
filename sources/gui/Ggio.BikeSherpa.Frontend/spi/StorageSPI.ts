import { Observable } from "@legendapp/state";
import { HubConnection } from "@microsoft/signalr";
import {ResourceNotification} from "@/infra/notification/Notification";

export interface IStorageContext<T> {
    getStore(): Observable<Record<T extends { id: number } ? number : string, T>>;
    forceRefresh(): Promise<void>;
    subscribeToOnErrorEvent(callback: (error: string) => void): string;
    unsubscribeFromOnErrorEvent(id: string): void;
}

export interface INotificationService {
    start(dataType: string): Promise<void>;
    stop(): Promise<void>;
    getConnection(): HubConnection | null;
    isConnected(): boolean;
    subscribeToDataType(dataType: string): Promise<void>;
    onNotification(handler: (notification: ResourceNotification) => void): () => void;
    onReconnected(callback: () => Promise<void>): () => void;
}