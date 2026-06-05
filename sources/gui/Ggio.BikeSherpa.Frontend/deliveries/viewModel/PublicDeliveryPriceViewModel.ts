import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";

export class PublicDeliveryPriceViewModel {
    private readonly publicDeliveryService: IPublicDeliveryService;

    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService,
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }
    
    public getVatRate = async () => {
        return await this.publicDeliveryService.getVatRate();
    }
}