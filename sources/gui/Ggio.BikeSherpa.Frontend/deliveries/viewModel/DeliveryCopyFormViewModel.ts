import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICustomerService} from "@/spi/CustomerSPI";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {inject} from "inversify";
import {deliveryFormBaseSchema, DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import Delivery from "@/deliveries/models/Delivery";
import * as Crypto from "expo-crypto";

export default class DeliveryCopyFormViewModel {
    private deliveryServices: IDeliveryServices;
    private customerServices: ICustomerService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.deliveryServices = deliveryServices;
        this.customerServices = customerServices;
    }

    public onSubmit = (delivery: DeliveryFormValues): void => {
        const customerCode = delivery.customerId;
        const customerId = this.customerServices.getCustomerIdByCode(customerCode);

        if (!customerId)
            return;
        delivery.customerId = customerId;

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
    }

    public getCopyDeliverySchema() {
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