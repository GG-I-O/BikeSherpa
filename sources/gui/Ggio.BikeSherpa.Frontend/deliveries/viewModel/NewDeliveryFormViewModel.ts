import {UseFormReset} from "react-hook-form";
import {IDeliveryServices} from "../spi/IDeliveryServices";
import {inject} from "inversify";
import {DeliveryServiceIdentifier} from "../bootstrapper/DeliveryServiceIdentifier";
import {deliveryFormBaseSchema, DeliveryFormValues} from "../models/zod/deliveryFormBaseSchema";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import * as Crypto from "expo-crypto";
import Delivery from "@/deliveries/models/Delivery";

export default class NewDeliveryFormViewModel {
    private deliveryServices: IDeliveryServices;
    private customerServices: ICustomerService;
    private resetCallback?: UseFormReset<DeliveryFormValues>;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.deliveryServices = deliveryServices;
        this.customerServices = customerServices;
    }

    // Keep it as a lambda to be able to use "this" on services.
    // Javascript does not bind "this" to the instance of class if declared as a method
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
        if (this.resetCallback)
            this.resetCallback();
    }

    public setResetCallback(reset?: UseFormReset<DeliveryFormValues>) {
        this.resetCallback = reset;
    }

    public getNewDeliverySchema() {
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