import {inject, injectable} from "inversify";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ILogger} from "@/spi/LogsSPI";
import {INotificationService} from "@/spi/StorageSPI";
import {ResourceNotification} from "@/infra/notification/Notification";
import {unassignedStepCount$} from "@/steps/store/UnassignedBadgeStore";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";
import {IUnassignedBadgeService} from "@/steps/spi/IUnassignedBadgeService";

/** SignalR group to subscribe to: same as the delivery store (storeName.toLocaleLowerCase()). */
const DELIVERY_DATA_TYPE = "delivery";
const REFRESH_DEBOUNCE_MS = 500;
const NOTIFICATION_RETRY_MS = 30000;

function toUtcDayIso(date: Date): string {
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0)).toISOString();
}

/**
 * Keeps the "unassigned steps" count up to date, completely independently
 * from the synced delivery store
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
        unassignedStepCount$.set(0);
    }

    public async refreshNow(): Promise<void> {
        const seq = ++this.requestSeq;
        try {
            const deliveries = await this.client.GetAllUnassignedDeliveriesEndpoint(toUtcDayIso(new Date()));

            // A newer refresh (or stop()) superseded this one: ignore the stale response
            if (seq !== this.requestSeq) return;

            unassignedStepCount$.set(
                deliveries.reduce((total, delivery) => total + (delivery.steps?.length ?? 0), 0)
            );
        } catch (error) {
            this.logger.warn('Failed to refresh unassigned steps count', error);
        }
    }

    private readonly handleNotification = (notification: ResourceNotification): void => {
        if (notification.dataType?.toLowerCase() !== DELIVERY_DATA_TYPE) return;
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