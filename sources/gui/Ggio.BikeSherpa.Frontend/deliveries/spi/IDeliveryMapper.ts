import Delivery, {DeliveryDto} from "@/deliveries/models/Delivery";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {DeliveryDisplayForCourier} from "@/deliveries/models/DeliveryDisplayForCourier";

export default interface IDeliveryMapper {
    DeliveryDtoToDelivery(deliveryDto: DeliveryDto): Delivery;
    DeliveryToDeliveryToDisplay(delivery: Delivery): DeliveryToDisplay;
    DeliveryToDeliveryDisplayForCourier(delivery: Delivery): DeliveryDisplayForCourier;
}