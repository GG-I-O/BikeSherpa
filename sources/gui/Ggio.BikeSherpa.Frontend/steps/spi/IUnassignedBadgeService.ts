export interface IUnassignedBadgeService {
    start(): void;
    stop(): void;
    refreshNow(): Promise<void>;
}