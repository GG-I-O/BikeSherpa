export interface IBackendClient<T> {
    GetAllEndpoint(lastSync?: string): Promise<T[]>;
    GetEndpoint(id: string): Promise<T | null>;
    AddEndpoint(item: T): Promise<string>;
    UpdateEndpoint(item: T): Promise<void>;
    DeleteEndpoint(item: T): Promise<void>;
}