import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICustomerService} from "@/spi/CustomerSPI";
import {deliveryFormBaseSchema, DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {inject} from "inversify";
import Delivery from "@/deliveries/models/Delivery";
import * as Crypto from "expo-crypto";

export default class DeliveryEditFormViewModel {
    private deliveryServices: IDeliveryServices;
    private customerServices: ICustomerService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.deliveryServices = deliveryServices;
        this.customerServices = customerServices;
    }

    // Keep it as a lambda to be able to use "this". Don't ask me why, JavaScript things
    public onSubmit = (delivery: DeliveryFormValues, oldDelivery: Delivery): void => {
        const customerCode = delivery.customerId;
        const customerId = this.customerServices.getCustomerIdByCode(customerCode);

        if (!customerId)
            return;
        delivery.customerId = customerId;

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
                        stepAddress: step.stepAddress,
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
                    distance: 0,
                    attachmentFilePaths: [],
                    realDeliveryDate: null,
                    estimatedDeliveryDate: delivery.startDate
                }
            })
        };

        this.deliveryServices.updateDelivery(deliveryObject);
    }

    public getEditDeliverySchema() {
        // Asking user to input the customer code, so that's what we validate
        // We convert it onsubmit later
        const customerList = Object.values(
            this.customerServices.getCustomerList$().get()
        );

        return deliveryFormBaseSchema.extend({
            customerId: deliveryFormBaseSchema.shape.code.refine(
                (value) => customerList.some(
                    (customer) => customer.code === value
                ),
                "Code client inexistant"
            )
        });
    }
}