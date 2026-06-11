import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";

export default class PublicDeliveryDetailsFormViewModel {
    private readonly publicDeliveryService: IPublicDeliveryService;

    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService,
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }

    public getUrgenciesLastHourToOrder = async () => {
        return await this.publicDeliveryService.getUrgenciesLastHourToOrder();
    }
    
    public getLastHourToOrder = async () => {
        return await this.publicDeliveryService.getLastHourToOrder();
    }
    
    public getWorkHours = async () => {
        return await this.publicDeliveryService.getWorkHours();
    }
}