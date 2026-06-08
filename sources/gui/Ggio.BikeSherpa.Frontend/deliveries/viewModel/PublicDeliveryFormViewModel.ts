import Delivery from "@/deliveries/models/Delivery";
import * as Crypto from "expo-crypto";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import {inject} from "inversify";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import Customer from "@/customers/models/Customer";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import PublicDeliveryEstimatedValue from "@/deliveries/models/PublicDeliveryEstimatedValue";

export default class PublicDeliveryFormViewModel {
    private readonly publicDeliveryService: IPublicDeliveryService;
    
    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService,
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }

    public onSubmit = async (delivery: PublicDeliveryFormValues): Promise<boolean> => {
        const {deliveryObject, customerObject} = this.mapFormValuesToObject(delivery);
        
        const result = await this.publicDeliveryService.createDelivery(deliveryObject, customerObject);
        if (result) {
            publicDeliveryStore$.delivery.set(deliveryObject);
            publicDeliveryStore$.customer.set({
                code: '',
                name: customerObject.name,
                email: customerObject.email,
                deliveryType: deliveryObject.pricingStrategy
            });
        }
        return result;
    }
    
    public getEstimatedValue = async (delivery: PublicDeliveryFormValues): Promise<PublicDeliveryEstimatedValue> => {
        const {deliveryObject} = this.mapFormValuesToObject(delivery);
        return await this.publicDeliveryService.getEstimatedValue(deliveryObject);
    }
    
    private mapFormValuesToObject = (delivery: PublicDeliveryFormValues): {deliveryObject: Delivery, customerObject: Customer} => {
        
        const deliveryObject: Delivery = {
            ...delivery,
            code: "",
            customerReference: "",
            contractDate: new Date().toISOString(),
            customerId: Crypto.randomUUID(),
            details: [""],
            discount: 0,
            extraCost: 0,
            status: 0,
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            steps: delivery.steps.map(step => {
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

        const customerObject: Customer = {
            id: Crypto.randomUUID(),
            code: "XXX",
            name: delivery.customer.name,
            email: delivery.customer.email,
            phoneNumber: delivery.customer.phoneNumber ?? '06 00 00 00 00',
            defaultDeliveryType: 0,
            address: {
                ...delivery.customer.address,
                name: delivery.customer.name ?? '',
                phone: delivery.customer.phoneNumber ?? '06 00 00 00 00'
            },
            siret: null,
            vatNumber: null
        }
        
        return {deliveryObject, customerObject};
    }
}