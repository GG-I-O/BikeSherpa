import Customer from "@/customers/models/Customer";
import InputCustomer from "@/customers/models/InputCustomer";
import CustomerServices from "@/customers/services/CustomerServices";
import { createRandomCustomer, createRandomInputCustomer, linkType } from "@/fixtures/customer-fixtures";
import { ICustomerService } from "@/spi/CustomerSPI"
import { ILogger } from "@/spi/LogsSPI";
import { IStorageContext } from "@/spi/StorageSPI";
import { faker } from "@faker-js/faker";
import { Observable, observable } from "@legendapp/state";
import * as Crypto from "expo-crypto";
import { mock } from "ts-jest-mocker"

const logger = mock<ILogger>();
const storage = mock<IStorageContext<Customer>>();

describe("CustomerServices", () => {
    let mockCustomerStore$: Observable<Record<string, Customer>>;
    let customerService: ICustomerService;
    let mocCustomers: Customer[];
    beforeEach(() => {
        mocCustomers = faker.helpers.multiple(() => createRandomCustomer(true, linkType.none), {
            count: 2,
        });
        mockCustomerStore$ = observable<Record<string, Customer>>({
            [mocCustomers[0].id]: mocCustomers[0],
            [mocCustomers[1].id]: mocCustomers[1]
        });
        storage.getStore.mockReturnValue(mockCustomerStore$);
        logger.extend.mockReturnValue(logger);
        customerService = new CustomerServices(logger, storage);
    });

    it("getCustomerList$ returns a customer list", () => {
        //arrange

        //act
        const customerList = customerService.getCustomerList$();

        //assert
        expect(customerList).not.toBeNull();
        expect(customerList).toBe(mockCustomerStore$);
    })

    it("getCustomer$ returns a customer", () => {
        //arrange

        //act
        const customer = customerService.getCustomer$(mocCustomers[0].id);

        //assert
        expect(customer).not.toBeNull();
        expect(customer).toBe(mockCustomerStore$[mocCustomers[0].id]);
    })

    it("deleteCustomer throws an error when no link is present", () => {
        //arrange

        //act

        //assert
        expect(() => customerService.deleteCustomer(mocCustomers[0].id)).toThrow(`Cannot delete the customer ${mocCustomers[0].id}`);
    })

    it("deleteCustomer deletes a customer when links exist", () => {
        //arrange
        mockCustomerStore$[mocCustomers[0].id].links.set([
            {
                rel: "delete",
                href: `/api/customers/${mocCustomers[0].id}`,
                method: "DELETE"
            }
        ])

        //act
        customerService.deleteCustomer(mocCustomers[0].id);

        //assert
        expect(mockCustomerStore$.peek()[mocCustomers[0].id]).toBeUndefined();
    })

    it("createCustomer creates a customer", () => {
        //arrange
        const newCustomer: InputCustomer = createRandomInputCustomer();
        jest.spyOn(Crypto, 'randomUUID').mockReturnValue("789");

        //act
        customerService.createCustomer(newCustomer);

        //assert
        const newCustomerPeek = mockCustomerStore$.peek()["789"];
        expect(newCustomerPeek).not.toBeUndefined();
        expect(newCustomerPeek.name).toBe(newCustomer.name);
        expect(newCustomerPeek.code).toBe(newCustomer.code);
    })

    it("updateCustomer updates a customer when links exist", () => {
        //arrange
        mockCustomerStore$[mocCustomers[0].id].links.set([
            {
                rel: "update",
                href: `/api/customers/${mocCustomers[0].id}`,
                method: "PUT"
            }
        ])

        const customerToUpdate: Customer = {
            ...createRandomCustomer(true, linkType.none),
            id: mocCustomers[0].id
        };

        //act
        customerService.updateCustomer(customerToUpdate);
        const updatedCustomerPeek = mockCustomerStore$.peek()[mocCustomers[0].id];

        //assert
        expect(updatedCustomerPeek.name).toBe(customerToUpdate.name);
        expect(updatedCustomerPeek.code).toBe(customerToUpdate.code);
    })

    it("updateCustomer does not update a customer when there is no link", () => {
        //arrange
        const customerToUpdate: Customer = {
            ...createRandomCustomer(true, linkType.none),
            id: mocCustomers[0].id
        };
        //act

        //assert
        expect(() => customerService.updateCustomer(customerToUpdate)).toThrow(`Cannot update customer ${customerToUpdate.id}`);
    })
})