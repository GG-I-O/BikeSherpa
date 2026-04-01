import { UseFormReset } from "react-hook-form";
import InputDelivery from "../models/InputDelivery";
import { IDeliveryServices } from "../spi/IDeliveryServices";
import { inject } from "inversify";
import { DeliveryServiceIdentifier } from "../bootstrapper/DeliveryServiceIdentifier";
import { deliveryFormBaseSchema } from "../models/zod/deliveryFormBaseSchema";

export default class NewDeliveryFormViewModel {
    private deliveryServices: IDeliveryServices;
    private resetCallback?: UseFormReset<InputDelivery>;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices
    ) {
        this.deliveryServices = deliveryServices;
    }

    public onSubmit(delivery: InputDelivery): void {
        this.deliveryServices.createDelivery(delivery);
        if (this.resetCallback)
            this.resetCallback();
    }

    public setResetCallback(reset?: UseFormReset<InputDelivery>) {
        this.resetCallback = reset;
    }

    public getNewDeliverySchema() {
        return deliveryFormBaseSchema;
    }
}