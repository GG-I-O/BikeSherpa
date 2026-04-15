import Delivery, { DeliveryDto } from "../models/Delivery";

export default class DeliveryMapper {
    public static DeliveryDtoToDelivery(deliveryDto: DeliveryDto) {
        const deliveryCrud = deliveryDto.data;
        let delivery: Delivery = {
            ... deliveryCrud,
            links: deliveryDto.links ?? []
        }
        return delivery;
    }
}