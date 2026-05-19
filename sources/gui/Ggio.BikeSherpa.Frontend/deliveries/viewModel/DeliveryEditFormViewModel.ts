import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import Delivery from "@/deliveries/models/Delivery";
import * as Crypto from "expo-crypto";
import AbstractFormViewModel from "@/deliveries/viewModel/AbstractFormViewModel";

export default class DeliveryEditFormViewModel extends AbstractFormViewModel {

    // Keep it as a lambda to be able to use "this" on services.
    // JavaScript does not bind "this" to the instance of the class if declared as a method
    public onSubmit = (delivery: DeliveryFormValues, oldDelivery: Delivery): void => {
        // Mapping
        const deliveryObject: Delivery = {
            id: oldDelivery.id,
            operationId: oldDelivery.operationId,
            ...delivery,
            steps: delivery.steps.map(step => {
                const oldStep = oldDelivery.steps.find(s => s.id === step.id);
                
                if (oldStep) {
                    const stepDate = new Date(oldStep.estimatedDeliveryDate);
                    const deliveryDate = new Date(delivery.startDate);
                    stepDate.setDate(deliveryDate.getDate());
                    stepDate.setMonth(deliveryDate.getMonth());
                    stepDate.setFullYear(deliveryDate.getFullYear());
                    return {
                        ... oldStep,
                        estimatedDeliveryDate: stepDate.toISOString(),
                        stepType: step.stepType,
                        stepAddress: {
                            ...step.stepAddress,
                            name: step.contactName ?? '',
                            phone: step.contactPhone ?? null
                        },
                        comment: step.comment ?? null
                    }
                }
                
                return {
                    ...step,
                    id: Crypto.randomUUID(),
                    order: 0,
                    completed: false,
                    stepZone: {name: '', cities: []},
                    courierId: null,
                    comment: step.comment ?? null,
                    courierComment: step.courierComment ?? null,
                    distance: 0,
                    attachmentFilePaths: [],
                    realDeliveryDate: null,
                    estimatedDeliveryDate: delivery.startDate,
                    stepAddress: {
                        ...step.stepAddress,
                        name: step.contactName ?? '',
                        phone: step.contactPhone ?? null
                    }
                }
            })
        };

        this.deliveryServices.updateDelivery(deliveryObject);
    }
}