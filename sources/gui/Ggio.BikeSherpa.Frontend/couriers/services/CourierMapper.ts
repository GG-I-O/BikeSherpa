import Courier, {CourierDto} from "../models/Courier";
import {CourierFormValues} from "@/couriers/viewModels/zod/courierFormBaseSchema";
import InputCourier from "@/couriers/models/InputCourier";

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

    public static CourierFormValuesToInputCourier(courier: CourierFormValues): InputCourier {
        return {
            ...courier,
            address: {
                ...courier.address,
                fullAddress: `${courier.address.streetInfo} ${courier.address.postcode} ${courier.address.city}`,
                name: `${courier.firstName} ${courier.lastName}`,
                phone: courier.phoneNumber
            }
        };
    }
}

export default CourierMapper;