import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import Delivery from "@/deliveries/models/Delivery";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";

export default class DeliveryDetailViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly deliveryMapper: IDeliveryMapper;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(DeliveryServiceIdentifier.Mapper) deliveryMapper: IDeliveryMapper
    ) {
        this.deliveryServices = deliveryServices;
        this.deliveryMapper = deliveryMapper;
    }

    public getDelivery = (id: string): DeliveryToDisplay => {
        const delivery: Delivery = this.deliveryServices.getDelivery$(id).get();

        return this.deliveryMapper.DeliveryToDeliveryToDisplay(delivery);
    }
}