import Delivery from "@/deliveries/models/Delivery";
import * as Crypto from "expo-crypto";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import {inject} from "inversify";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import Customer from "@/customers/models/Customer";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import PublicDeliveryEstimatedValue from "@/deliveries/models/PublicDeliveryEstimatedValue";

export default class PublicDeliveryFormViewModel {
    private readonly publicDeliveryService: IPublicDeliveryService;
    
    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService,
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }

    public onSubmit = async (delivery: PublicDeliveryFormValues, customerType: PublicDeliveryCustomerTypeEnum): Promise<boolean> => {
        const {deliveryObject, customerObject} = this.mapFormValuesToObject(delivery, customerType);
        
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
    
    public getEstimatedValue = async (delivery: PublicDeliveryFormValues, customerType: PublicDeliveryCustomerTypeEnum): Promise<PublicDeliveryEstimatedValue> => {
        const {deliveryObject} = this.mapFormValuesToObject(delivery, customerType);
        return await this.publicDeliveryService.getEstimatedValue(deliveryObject);
    }
    
    private mapFormValuesToObject = (delivery: PublicDeliveryFormValues, customerType: PublicDeliveryCustomerTypeEnum): {deliveryObject: Delivery, customerObject: Customer} => {
        if (customerType === PublicDeliveryCustomerTypeEnum.Sender) {
            delivery.customer.address = delivery.steps[0].stepAddress;
            delivery.steps[0].contactName = delivery.customer.name;
            delivery.steps[0].contactPhone = delivery.customer.phoneNumber;
        }
        if (customerType === PublicDeliveryCustomerTypeEnum.Receiver) {
            delivery.customer.address = delivery.steps[1].stepAddress;
            delivery.steps[1].contactName = delivery.customer.name;
            delivery.steps[1].contactPhone = delivery.customer.phoneNumber;
        }
        
        const deliveryObject: Delivery = {
            ...delivery,
            code: "",
            contractDate: "",
            customerId: "",
            details: [],
            discount: 0,
            distance: 0,
            extraCost: 0,
            reportId: "",
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
            phoneNumber: delivery.customer.phoneNumber ?? '',
            address: {
                ...delivery.customer.address,
                name: delivery.customer.name ?? '',
                phone: delivery.customer.phoneNumber ?? ''
            },
            siret: null,
            vatNumber: null
        }
        
        return {deliveryObject, customerObject};
    }
}