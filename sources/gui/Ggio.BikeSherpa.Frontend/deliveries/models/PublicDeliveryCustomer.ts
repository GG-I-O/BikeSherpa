import {Address} from "@/models/Address";

type PublicDeliveryCustomer = {
    email: string,
    code: string,
    name: string,
    deliveryType: number,
    address: Address
};

export default PublicDeliveryCustomer;