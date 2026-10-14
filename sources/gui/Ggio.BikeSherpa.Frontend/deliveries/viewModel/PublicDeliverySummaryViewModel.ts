import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import Delivery from "@/deliveries/models/Delivery";
import Customer from "@/customers/models/Customer";
import {PublicDeliveryCustomerFormValues} from "@/deliveries/models/zod/publicDeliveryCustomerFormBaseSchema";
import * as Crypto from "expo-crypto";

export default class PublicDeliverySummaryViewModel {
    private readonly publicDeliveryService: IPublicDeliveryService;

    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService,
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }
    
    public createDelivery = async (delivery: Delivery, customer: Customer): Promise<boolean> => {
        return await this.publicDeliveryService.createDelivery(delivery, customer);
    }
    
    public mapFormValuesToObject(customer: PublicDeliveryCustomerFormValues): Customer {
        return {
            id: Crypto.randomUUID(),
            code: customer.code,
            name: customer.name !== '' ? customer.name : customer.address.name,
            email: customer.email,
            phoneNumber: customer.phoneNumber ?? '06 00 00 00 00',
            defaultDeliveryType: 0,
            address: {
                ...customer.address,
                name: customer.address.name ?? '',
                phone: customer.phoneNumber ?? '06 00 00 00 00'
            },
            siret: null,
            vatNumber: null
        }
    }
}