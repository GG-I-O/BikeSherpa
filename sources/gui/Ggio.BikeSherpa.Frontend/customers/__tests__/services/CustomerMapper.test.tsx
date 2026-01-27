import { CustomerDto } from "@/customers/models/Customer"
import CustomerMapper from "@/customers/services/CustomerMapper";
import { createRandomCustomerDto } from "@/fixtures/customer-fixtures";

describe("CustomerMapper", () => {
    it("CustomerMapper converts a CustomerDto into a Customer", () => {
        //arrange
        const customerDto: CustomerDto = {
            data: {
                name: "Customer",
                code: "CUS",
                siret: null,
                vatNumber: null,
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
        expect(customer.name).toBe(customerDto.data.name);
        expect(customer.links).toStrictEqual([]);
        expect(customer.address).toStrictEqual(customerDto.data.address);
    })
})