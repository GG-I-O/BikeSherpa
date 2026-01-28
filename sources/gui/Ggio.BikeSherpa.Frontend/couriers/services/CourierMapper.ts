import Courier, { CourierDto } from "../models/Courier";

class CourierMapper {
    public static CourierDtoToCourier(courierDto: CourierDto) {
        const courierCrud = courierDto.data;
        const address = courierCrud.address;
        let courier: Courier = {
            ...courierCrud,
            address: {
                fullAddress: `${address.streetInfo} ${address.postcode} ${address.city}`,
                ...address
            },
            links: courierDto.links ?? []
        };
        return courier;
    }
}

export default CourierMapper;