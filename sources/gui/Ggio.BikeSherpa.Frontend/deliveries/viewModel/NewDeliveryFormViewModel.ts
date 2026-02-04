import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import { IDeliveryService } from "@/spi/DeliverySPI";
import { InputDelivery } from "../models/InputDelivery";
import { Delivery } from "../models/Delivery";
import { deliveryFormBaseSchema } from "./zod/deliveryFormBaseSchema";

export default class NewDeliveryFormViewModel {
    private deliveryServices: IDeliveryService;
    private resetCallback?: UseFormReset<InputDelivery>;

    constructor(
        @inject(ServicesIdentifiers.DeliveryServices) deliveryServices: IDeliveryService
    ) {
        this.deliveryServices = deliveryServices;
    }

    public onSubmit = (delivery: InputDelivery): void => {
        this.deliveryServices.createDelivery(delivery);
        if (this.resetCallback) {
            this.resetCallback(); // Clear form after submission
        }
    };

    public setResetCallback(reset?: UseFormReset<InputDelivery>) {
        this.resetCallback = reset;
    }

    public getNewDeliverySchema(deliveryList: Delivery[]) {
        return deliveryFormBaseSchema
            .extend({
                code: deliveryFormBaseSchema.shape.code.refine((value: string) => !deliveryList.some((delivery) => delivery.code === value), "Le code doit être unique")
            });
    }
}