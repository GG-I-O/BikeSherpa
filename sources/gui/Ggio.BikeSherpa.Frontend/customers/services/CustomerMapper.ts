import Customer, { CustomerDto } from "../models/Customer";

class CustomerMapper {
    public static CustomerDtoToCustomer(customerDto: CustomerDto) {
        const customerCrud = customerDto.data;
        const address = customerCrud.address;
        let customer: Customer = {
            ...customerCrud,
            address: {
                fullAddress: `${address.streetInfo} ${address.postcode} ${address.city}`,
                ...address
            },
            links: customerDto.links ?? []
        };
        return customer;
    }
}

export default CustomerMapper;