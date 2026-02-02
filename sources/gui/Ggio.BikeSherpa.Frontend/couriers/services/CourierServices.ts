import { Observable } from "@legendapp/state";
import * as Crypto from "expo-crypto";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import Courier from "../models/Courier";
import InputCourier from "../models/InputCourier";
import { inject, injectable } from "inversify";
import { IStorageContext } from "@/spi/StorageSPI";
import { ICourierService } from "@/spi/CourierSPI";
import { hateoasRel } from "@/models/HateoasLink";

@injectable()
export default class CourierServices implements ICourierService {
    private logger: ILogger;
    private storage: IStorageContext<Courier>;
    private readonly courierStore$: Observable<Record<string, Courier>>;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.CourierStorage) courierStorage: IStorageContext<Courier>) {
        this.logger = logger;
        this.logger = this.logger.extend("Courier");
        this.storage = courierStorage;
        this.courierStore$ = this.storage.getStore();
    }

    /**
     * Subscribe to an Observable of courier list
     */
    public getCourierList$(): Observable<Record<string, Courier>> {
        return this.courierStore$;
    }

    /**
     * Subscribe to an Observable of a single courier
     */
    public getCourier$(courierId: string): Observable<Courier> {
        return this.courierStore$[courierId];
    }

    /**
     * Get a single courier without subscribing to changes
     */
    private getCourier(courierId: string): Courier {
        return this.courierStore$[courierId].peek();
    }

    public deleteCourier(courierId: string): void {
        const courier = this.getCourier(courierId);
        const canDelete = courier.links?.some((link) => link.rel === hateoasRel.delete);

        if (!canDelete)
            throw new Error(`Cannot delete the courier ${courierId}`);
        this.courierStore$[courierId].delete();
    }

    // Wrapper for NewCourierForm
    public createCourier(courier: InputCourier) {
        const newCourier: Courier = {
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            ...courier,
        };
        this.courierStore$[newCourier.id].set(newCourier);
    };

    // Wrapper for EditCourierForm
    public updateCourier(courier: Courier) {
        const courierToUpdate = this.getCourier(courier.id);
        const canUpdate = courierToUpdate.links?.some((link) => link.rel === hateoasRel.update);
        if (!canUpdate)
            throw new Error(`Cannot update courier ${courier.id}`);
        this.courierStore$[courier.id].assign(courier);
    };
}