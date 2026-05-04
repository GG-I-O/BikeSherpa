import {UseFormReset} from "react-hook-form";
import {DeliveryFormValues} from "../models/zod/deliveryFormBaseSchema";
import * as Crypto from "expo-crypto";
import Delivery from "@/deliveries/models/Delivery";
import AbstractFormViewModel from "@/deliveries/viewModel/AbstractFormViewModel";

export default class NewDeliveryFormViewModel extends AbstractFormViewModel {
    private resetCallback?: UseFormReset<DeliveryFormValues>;

    // Keep it as a lambda to be able to use "this" on services.
    // JavaScript does not bind "this" to the instance of the class if declared as a method
    public onSubmit = (delivery: DeliveryFormValues): void => {
        // Mapping
        const deliveryObject: Delivery = {
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            ...delivery,
            steps: delivery.steps.map(step => {
                return {
                    ...step,
                    id: Crypto.randomUUID(),
                    order: 0,
                    completed: false,
                    stepZone: {name: '', cities: []},
                    courierId: null,
                    comment: step.comment ?? null,
                    distance: 0,
                    attachmentFilePaths: [],
                    realDeliveryDate: null,
                    estimatedDeliveryDate: delivery.startDate
                }
            })
        };

        this.deliveryServices.createDelivery(deliveryObject);
        if (this.resetCallback)
            this.resetCallback();
    }

    public setResetCallback(reset?: UseFormReset<DeliveryFormValues>) {
        this.resetCallback = reset;
    }
}