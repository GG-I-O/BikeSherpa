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

    // Keep it as a lambda to be able to use "this". Don't ask me why, JavaScript things
    public onSubmit = (delivery: DeliveryFormValues): void => {
        // Generate code without increment
        const date = new Date(delivery.contractDate);
        const day = date.getDate().toLocaleString(undefined, {minimumIntegerDigits: 2, useGrouping: false});
        const month = (date.getMonth() + 1).toLocaleString(undefined, {minimumIntegerDigits: 2, useGrouping: false});
        const code = `${delivery.customerId}-${day}${month}`;

        try {
            // Search other deliveries with same code to apply increment
            const deliveryList = Object.values(
                this.deliveryServices.getDeliveryList$().get()
            );
            const deliveriesWithSameCode = deliveryList.filter(
                (delivery) => delivery.code.startsWith(code)
            );

            // Apply increment
            const increments = deliveriesWithSameCode.map(
                (deliveries) => deliveries.code.split("-")[2]
            );

            let newIncrement = 1;
            if (increments.length > 0) {
                increments.sort();
                newIncrement = parseInt(increments[increments.length - 1]) + 1;
            }
            delivery.code = code + "-" + newIncrement;

            // Convert customerCode to customerId
            const customerList = Object.values(
                this.customerServices.getCustomerList$().get()
            );
            const customer = customerList.find(
                (customer) => customer.code === delivery.customerId
            );
            delivery.customerId = customer!.id;
        } catch (e) {
            console.error("Create Delivery onSubmit found :" + delivery.customerId);
        }

        // Mapping
        delivery.steps.map(step => {
            return {
                ...step,
                id: Crypto.randomUUID(),
            }
        });

        const deliveryObject: Delivery = {
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            ...delivery,
            steps: delivery.steps.map(step => {
                return {
                    ...step,
                    id: Crypto.randomUUID(),
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