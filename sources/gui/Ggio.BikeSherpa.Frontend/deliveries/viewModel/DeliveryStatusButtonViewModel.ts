import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";

export default class DeliveryStatusButtonViewModel { 
    private readonly service: IDeliveryServices;
    
    public constructor(
        @inject(DeliveryServiceIdentifier.Services) service: IDeliveryServices
    ) {
        this.service = service;
    }
    
    public changeStatusToNew = (deliveryId: string) => {
        this.service.updateDeliveryStatusToNew(deliveryId);   
    }
    
    public changeStatusToPending = (deliveryId: string) => {
        this.service.updateDeliveryStatusToPending(deliveryId);
    }
}