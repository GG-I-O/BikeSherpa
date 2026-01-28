import { CustomerDto } from "@/customers/models/Customer"
import CustomerMapper from "@/customers/services/CustomerMapper";
import { createRandomCustomerDto } from "@/fixtures/customer-fixtures";

describe("CustomerMapper", () => {
    it("CustomerMapper converts a CustomerDto into a Customer", () => {
        //arrange
        const customerDto: CustomerDto = createRandomCustomerDto();

        //act
        const customer = CustomerMapper.CustomerDtoToCustomer(customerDto);

        //assert
        expect(customer.name).toBe(customerDto.data.name);
        expect(customer.links).toStrictEqual([]);
        expect(customer.address).toStrictEqual(customerDto.data.address);
    })
})