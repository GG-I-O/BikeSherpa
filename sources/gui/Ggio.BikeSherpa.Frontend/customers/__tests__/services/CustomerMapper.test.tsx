import { CustomerDto } from "@/customers/models/Customer"
import CustomerMapper from "@/customers/services/CustomerMapper";

describe("CustomerMapper", () => {
    it("CustomerMapper converts a CustomerDto into a Customer", () => {
        //arrange
        const customerDto: CustomerDto = {
            data: {
                name: "Customer",
                code: "CUS",
                siret: null,
                email: "customer@gmail.com",
                phoneNumber: "0609080544",
                address: {
                    name: "Customer",
                    streetInfo: "10 rue de la Paix",
                    complement: null,
                    postcode: "75000",
                    city: "Paris"
                },
                createdAt: "123465689741",
                updatedAt: "123465689798",
                id: "123"
            },
            links: null
        };

        //act
        const customer = CustomerMapper.CustomerDtoToCustomer(customerDto);

        //assert
        expect(customer.name).toBe("Customer");
        expect(customer.links).toStrictEqual([]);
        expect(customer.address).toStrictEqual({
            name: "Customer",
            fullAddress: "10 rue de la Paix 75000 Paris",
            streetInfo: "10 rue de la Paix",
            complement: null,
            postcode: "75000",
            city: "Paris"
        });
    })
})