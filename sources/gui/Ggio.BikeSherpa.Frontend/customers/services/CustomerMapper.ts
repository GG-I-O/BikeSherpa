import Customer, {CustomerDto} from "../models/Customer";
import {CustomerFormValues} from "@/customers/viewModels/zod/customerFormBaseSchema";
import InputCustomer from "@/customers/models/InputCustomer";

class CustomerMapper {
    public static CustomerDtoToCustomer(customerDto: CustomerDto) {
        const customerCrud = customerDto.data;
        const address = customerCrud.address;
        let customer: Customer = {
            ...customerCrud,
            defaultDeliveryType: customerCrud.defaultDeliveryType ?? 0,
            address: {
                fullAddress: `${address.streetInfo} ${address.postcode} ${address.city}`,
                ...address
            },
            links: customerDto.links ?? []
        };
        return customer;
    }
    
    public static CustomerFormValuesToInputCustomer(customer: CustomerFormValues): InputCustomer {
        return {
            ...customer,
            defaultDeliveryType: customer.defaultDeliveryType ?? 0,
            address: {
                ...customer.address,
                fullAddress: `${customer.address.streetInfo} ${customer.address.postcode} ${customer.address.city}`,
                name: customer.name,
                phone: customer.phoneNumber
            }
        };
    }
}

export default CustomerMapper;