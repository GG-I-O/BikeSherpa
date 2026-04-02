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

    // Keep it as a lambda to be able to use "this". Don't ask me why, JavaScript things
    public onSubmit = (delivery: InputDelivery): void => {
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