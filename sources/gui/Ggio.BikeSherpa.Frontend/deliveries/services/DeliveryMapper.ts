import { Delivery, DeliveryDto } from "../models/Delivery";

class DeliveryMapper {
    public static DeliveryrDtoToDelivery(deliveryDto: DeliveryDto) {
        const deliveryCrud = deliveryDto.data;
        const address = deliveryCrud.address;
        let delivery: Delivery = {
            ...deliveryCrud,
            address: {
                fullAddress: `${address.streetInfo} ${address.postcode} ${address.city}`,
                ...address
            },
            links: deliveryDto.links ?? []
        };
        return delivery;
    }
}

export default DeliveryMapper;