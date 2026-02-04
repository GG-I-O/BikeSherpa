import { Observable } from "@legendapp/state";
import * as Crypto from "expo-crypto";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import { Delivery } from "../models/Delivery";
import { InputDelivery } from "../models/InputDelivery";
import { inject, injectable } from "inversify";
import { IStorageContext } from "@/spi/StorageSPI";
import { IDeliveryService } from "@/spi/DeliverySPI";
import { hateoasRel } from "@/models/HateoasLink";

@injectable()
export default class DeliveryServices implements IDeliveryService {
    private logger: ILogger;
    private storage: IStorageContext<Delivery>;
    private readonly deliveryStore$: Observable<Record<string, Delivery>>;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.DeliveryStorage) deliveryStorage: IStorageContext<Delivery>) {
        this.logger = logger;
        this.logger = this.logger.extend("Delivery");
        this.storage = deliveryStorage;
        this.deliveryStore$ = this.storage.getStore();
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
    public createDelivery(delivery: InputDelivery) {
        const newDelivery: Delivery = {
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            ...delivery,
        };
        this.deliveryStore$[newDelivery.id].set(newDelivery);
    };

    // Wrapper for EditDeliveryForm
    public updateDelivery(delivery: Delivery) {
        const deliveryToUpdate = this.getDelivery(delivery.id);
        const canUpdate = deliveryToUpdate.links?.some((link) => link.rel === hateoasRel.update);
        if (!canUpdate)
            throw new Error(`Cannot update delivery ${delivery.id}`);
        this.deliveryStore$[delivery.id].assign(delivery);
    };
}