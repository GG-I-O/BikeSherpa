import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { Delivery } from "../models/Delivery";
import { IDeliveryService } from "@/spi/DeliverySPI";
import { inject } from "inversify";
import * as zod from 'zod';
import { deliveryFormBaseSchema } from "./zod/deliveryFormBaseSchema";

export default class EditDeliveryFormViewModel {
    private deliveryServices: IDeliveryService;

    constructor(
        @inject(ServicesIdentifiers.DeliveryServices) deliveryServices: IDeliveryService
    ) {
        this.deliveryServices = deliveryServices;
    }

    onSubmit = (delivery: Delivery) => {
        this.deliveryServices.updateDelivery(delivery);
    };

    public getEditDeliverySchema(deliveryToEdit: Delivery, deliveryList: Delivery[]) {
        const originalCode = deliveryToEdit.code;

        return deliveryFormBaseSchema.extend({
            id: zod
                .string()
                .min(1),
            code: deliveryFormBaseSchema.shape.code.refine((value: string) => {
                if (originalCode === value) {
                    return true;
                }
                return !deliveryList.some((delivery) => delivery.code === value);
            }, "Le code doit être unique"),
        });
    }
}