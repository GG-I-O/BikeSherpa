import {inject, injectable} from "inversify";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ILogger} from "@/spi/LogsSPI";
import {INotificationService} from "@/spi/StorageSPI";
import {ResourceNotification} from "@/infra/notification/Notification";
import {seenUnassignedIds$, unassignedIds$} from "@/steps/store/UnassignedBadgeStore";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";
import {IUnassignedBadgeService} from "@/steps/spi/IUnassignedBadgeService";

/** Must match the delivery store's resource name, i.e. storeName.toLocaleLowerCase(). */
const DELIVERY_DATA_TYPE = "delivery";
const REFRESH_DEBOUNCE_MS = 500;
const NOTIFICATION_RETRY_MS = 30000;

function toUtcDayIso(date: Date): string {
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0)).toISOString();
}

/**
 * Keeps the "unassigned deliveries" badge up to date, completely independently
 * from the synced delivery store (which only holds the list of the active view).
 */
@injectable()
export class UnassignedBadgeService implements IUnassignedBadgeService {
    private readonly client: IDeliveryCustomBackendClientFacade;
    private readonly notificationService: INotificationService;
    private readonly logger: ILogger;

    private started = false;
    private requestSeq = 0;
    private disposers: (() => void)[] = [];
    private refreshTimer: ReturnType<typeof setTimeout> | null = null;
    private retryTimer: ReturnType<typeof setTimeout> | null = null;

    constructor(
        @inject(DeliveryServiceIdentifier.CustomBackendClientFacade) client: IDeliveryCustomBackendClientFacade,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService,
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.client = client;
        this.notificationService = notificationService;
        this.logger = logger.extend('UnassignedBadgeService');
    }

    /** Idempotent. */
    public start(): void {
        if (this.started) return;
        this.started = true;

        this.disposers.push(
            this.notificationService.onNotification(this.handleNotification),
            this.notificationService.onReconnected(() => this.refreshNow())
        );

        void this.refreshNow();
        void this.connectNotifications();
    }

    public stop(): void {
        this.started = false;
        this.requestSeq++; // invalidate any in-flight request
        this.disposers.forEach((dispose) => dispose());
        this.disposers = [];
        if (this.refreshTimer) clearTimeout(this.refreshTimer);
        if (this.retryTimer) clearTimeout(this.retryTimer);
        this.refreshTimer = null;
        this.retryTimer = null;
        unassignedIds$.set([]);
    }

    public async refreshNow(): Promise<void> {
        const seq = ++this.requestSeq;
        try {
            const deliveries = await this.client.GetAllUnassignedDeliveriesEndpoint(toUtcDayIso(new Date()));

            // A newer refresh (or stop()) superseded this one: ignore the stale response
            if (seq !== this.requestSeq) return;

            const ids = deliveries.map((delivery) => delivery.id);
            const idSet = new Set(ids);
            unassignedIds$.set(ids);

            // Prune seen ids that no longer exist (assigned / deleted) so the list doesn't grow forever
            const seen = seenUnassignedIds$.peek();
            const pruned = seen.filter((id) => idSet.has(id));
            if (pruned.length !== seen.length) {
                seenUnassignedIds$.set(pruned);
            }
        } catch (error) {
            // Best effort: a failed badge refresh must never break the app
            this.logger.warn('Failed to refresh unassigned deliveries badge', error);
        }
    }

    private readonly handleNotification = (notification: ResourceNotification): void => {
        if (notification.dataType?.toLowerCase() !== DELIVERY_DATA_TYPE.toLowerCase()) return;
        // Any operation can change the unassigned list (a PUT can assign/unassign, a DELETE removes)
        this.scheduleRefresh();
    };

    private scheduleRefresh(): void {
        if (this.refreshTimer) clearTimeout(this.refreshTimer);
        this.refreshTimer = setTimeout(() => {
            this.refreshTimer = null;
            void this.refreshNow();
        }, REFRESH_DEBOUNCE_MS);
    }

    private async connectNotifications(): Promise<void> {
        try {
            await this.notificationService.start(DELIVERY_DATA_TYPE);
            if (!this.started) return;
            // Catch anything that happened before the subscription was active
            this.scheduleRefresh();
        } catch (error) {
            if (!this.started) return;
            this.logger.warn(`Notifications unavailable, retrying in ${NOTIFICATION_RETRY_MS / 1000}s`, error);
            this.retryTimer = setTimeout(() => {
                void this.connectNotifications();
            }, NOTIFICATION_RETRY_MS);
        }
    }
}