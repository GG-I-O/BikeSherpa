import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {inject} from "inversify";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";

export default class PublicDeliveryLoginFormViewModel { 
    private readonly publicDeliveryService: IPublicDeliveryService;
    
    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }
    
    public login = async (email: string, code: string) => {
        return await this.publicDeliveryService.loginPublicDeliveryCustomer(email, code);
    }
}