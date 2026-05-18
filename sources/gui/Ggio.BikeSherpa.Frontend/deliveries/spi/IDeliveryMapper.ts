import Delivery, {DeliveryDto} from "@/deliveries/models/Delivery";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";

export default interface IDeliveryMapper {
    DeliveryDtoToDelivery(deliveryDto: DeliveryDto): Delivery;
    DeliveryToDeliveryToDisplay(delivery: Delivery): DeliveryToDisplay;
}