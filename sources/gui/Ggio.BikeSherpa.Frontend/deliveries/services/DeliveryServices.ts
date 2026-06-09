import { inject, injectable } from "inversify";
import { IDeliveryServices } from "../spi/IDeliveryServices";
import { ILogger } from "@/spi/LogsSPI";
import Delivery from "../models/Delivery";
import { IStorageContext } from "@/spi/StorageSPI";
import { Observable } from "@legendapp/state";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { DeliveryServiceIdentifier } from "../bootstrapper/DeliveryServiceIdentifier";
import { hateoasRel } from "@/models/HateoasLink";
import {IDeliveryStorageMiddleware} from "@/deliveries/spi/IDeliveryStorageMiddleware";
import deliveryOperationAction from "@/deliveries/data/deliveryOperationAction";
import {DeliveryStatusEnum} from "@/deliveries/data/deliveryStatusEnum";

@injectable()
export default class DeliveryServices implements IDeliveryServices {
    private logger: ILogger;
    private readonly storageMiddleware: IDeliveryStorageMiddleware;
    private storage: IStorageContext<Delivery>;
    private readonly deliveryStore$: Observable<Record<string, Delivery>>;
    
    private readonly deliveryStorageMiddleware: IDeliveryStorageMiddleware;
    
    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(DeliveryServiceIdentifier.StorageMiddleware) storageMiddleware: IDeliveryStorageMiddleware,
        @inject(DeliveryServiceIdentifier.Storage) deliveryStorage: IStorageContext<Delivery>,
        @inject(DeliveryServiceIdentifier.StorageMiddleware) deliveryStorageMiddleware: IDeliveryStorageMiddleware,
    ) {
        this.logger = logger;
        this.logger = this.logger.extend("Delivery");
        this.storageMiddleware = storageMiddleware;
        this.storage = deliveryStorage;
        this.deliveryStore$ = this.storage.getStore();
        
        this.deliveryStorageMiddleware = deliveryStorageMiddleware;
    }

    /**
         * Subscribe to an Observable of delivery list
         */
    public getDeliveryList$(): Observable<Record<string, Delivery>> {
        return this.deliveryStore$;
    }

    /**
     * Subscribe to an Observable of a single delivery
     */
    public getDelivery$(deliveryId: string): Observable<Delivery> {
        return this.deliveryStore$[deliveryId];
    }

    /**
     * Load deliveries with StorageMiddleware
     */
    public loadMyDeliveries(date: string | null): void {
        this.storageMiddleware.setGetAllDateForDailyDeliveries(date);
        this.storage.forceRefresh().then();
    }

    /**
     * Get a single delivery without subscribing to changes
     */
    private getDelivery(deliveryId: string): Delivery {
        return this.deliveryStore$[deliveryId].peek();
    }

    public deleteDelivery(deliveryId: string): void {
        const delivery = this.getDelivery(deliveryId);
        const canDelete = delivery.links?.some((link) => link.rel === hateoasRel.delete);

        if (!canDelete)
            throw new Error(`Cannot delete the delivery ${deliveryId}`);
        this.deliveryStore$[deliveryId].delete();
    }

    // Wrapper for NewDeliveryForm
    public createDelivery(delivery: Delivery) {
        this.deliveryStore$[delivery.id].set(delivery);
    };

    // Wrapper for EditDeliveryForm
    public updateDelivery(delivery: Delivery) {
        const deliveryToUpdate = this.getDelivery(delivery.id);
        const canUpdate = deliveryToUpdate.links?.some((link) => link.rel === hateoasRel.update);
        if (!canUpdate)
            throw new Error(`Cannot update delivery ${delivery.id}`);
        
        this.deliveryStore$[delivery.id].assign(delivery);
    };
    
    public updateDeliveryStatusToNew(deliveryId: string) {
        const deliveryObservable = this.getDelivery$(deliveryId);

        // Test if the user got the rights to do this action
        const delivery = deliveryObservable.get();
        if (!delivery.links || !delivery.links.some((link) => link.rel === hateoasRel.delivery.put.renew)) {
            this.logger.error(`Cannot update status to New for delivery ${deliveryId}`);
            return
        }

        this.deliveryStorageMiddleware.addUpdateDeliveryState(
            deliveryId,
            deliveryOperationAction.putRenew
        );

        deliveryObservable.status.set(DeliveryStatusEnum.New);
    };

    public updateDeliveryStatusToPending(deliveryId: string) {
        const deliveryObservable = this.getDelivery$(deliveryId);

        // Test if the user got the rights to do this action
        const delivery = deliveryObservable.get();
        if (!delivery.links || !delivery.links.some((link) => link.rel === hateoasRel.delivery.put.pending)) {
            this.logger.error(`Cannot update status to Pending for delivery ${deliveryId}`);
            return
        }

        this.deliveryStorageMiddleware.addUpdateDeliveryState(
            deliveryId,
            deliveryOperationAction.putPending
        );

        deliveryObservable.status.set(DeliveryStatusEnum.Pending);
    };
}