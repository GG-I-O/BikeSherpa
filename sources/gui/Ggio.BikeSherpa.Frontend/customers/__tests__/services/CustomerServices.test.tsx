import Customer from "@/customers/models/Customer";
import InputCustomer from "@/customers/models/InputCustomer";
import CustomerServices from "@/customers/services/CustomerServices";
import { ICustomerService } from "@/spi/CustomerSPI"
import { ILogger } from "@/spi/LogsSPI";
import { IStorageContext } from "@/spi/StorageSPI";
import { Observable, observable } from "@legendapp/state";
import * as Crypto from "expo-crypto";
import { mock } from "ts-jest-mocker"

const logger = mock<ILogger>();
const storage = mock<IStorageContext<Customer>>();

describe("CustomerServices", () => {
    let mockCustomerStore$: Observable<Record<string, Customer>>;
    let customerService: ICustomerService;
    beforeEach(() => {
        mockCustomerStore$ = observable<Record<string, Customer>>({
            "123": {
                id: "123",
                name: "Existing Company",
                address: {
                    name: "Existing Company",
                    fullAddress: "10 rue de la Paix 75000 Paris",
                    streetInfo: "10 rue de la Paix",
                    complement: undefined,
                    postcode: "75000",
                    city: "Paris"
                },
                code: "EX1",
                phoneNumber: "0609080704",
                email: "existing.company@gmail.com"
            },
            "456": {
                id: "456",
                name: "Another Existing Company",
                address: {
                    name: "Another Existing Company",
                    fullAddress: "20 rue de la Paix 75000 Paris",
                    streetInfo: "20 rue de la Paix",
                    complement: undefined,
                    postcode: "75000",
                    city: "Paris"
                },
                code: "EX2",
                phoneNumber: "0609080704",
                email: "existing.company@gmail.com"
            }
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
        const customer = customerService.getCustomer$("123");

        //assert
        expect(customer).not.toBeNull();
        expect(customer).toBe(mockCustomerStore$["123"]);
    })

    it("deleteCustomer throws an error when no link is present", () => {
        //arrange

        //act

        //assert
        expect(() => customerService.deleteCustomer("123")).toThrow("Cannot delete the customer 123");
    })

    it("deleteCustomer deletes a customer when links exist", () => {
        //arrange
        mockCustomerStore$["123"].links.set([
            {
                rel: "delete",
                href: "/api/customers/123",
                method: "DELETE"
            }
        ])
        //act
        customerService.deleteCustomer("123");

        //assert
        expect(mockCustomerStore$.peek()["123"]).toBeUndefined();
    })

    it("createCustomer creates a customer", () => {
        //arrange
        const newCustomer: InputCustomer = {
            name: "La Société",
            address: {
                name: "",
                fullAddress: "",
                streetInfo: "",
                complement: undefined,
                postcode: "",
                city: ""
            },
            code: "SOC",
            phoneNumber: "",
            email: ""
        }
        jest.spyOn(Crypto, 'randomUUID').mockReturnValue("789");

        //act
        customerService.createCustomer(newCustomer);

        //assert
        const newCustomerPeek = mockCustomerStore$.peek()["789"];
        expect(newCustomerPeek).not.toBeUndefined();
        expect(newCustomerPeek.name).toBe("La Société");
        expect(newCustomerPeek.code).toBe("SOC");
    })

    it("updateCustomer updates a customer when links exist", () => {
        //arrange
        mockCustomerStore$["123"].links.set([
            {
                rel: "update",
                href: "/api/customers/123",
                method: "PUT"
            }
        ])

        const customerToUpdate: Customer = {
            name: "La Société",
            address: {
                name: "",
                fullAddress: "",
                streetInfo: "",
                complement: undefined,
                postcode: "",
                city: ""
            },
            code: "SOC",
            phoneNumber: "",
            email: "",
            id: "123"
        }

        //act
        customerService.updateCustomer(customerToUpdate);
        const updatedCustomerPeek = mockCustomerStore$.peek()["123"];

        //assert
        expect(updatedCustomerPeek.name).toBe("La Société");
        expect(updatedCustomerPeek.code).toBe("SOC");
    })

    it("updateCustomer does not update a customer when the is no link", () => {
        //arrange
        const customerToUpdate: Customer = {
            name: "La Société",
            address: {
                name: "",
                fullAddress: "",
                streetInfo: "",
                complement: undefined,
                postcode: "",
                city: ""
            },
            code: "SOC",
            phoneNumber: "",
            email: "",
            id: "123"
        }
        //act

        //assert
        expect(() => customerService.updateCustomer(customerToUpdate)).toThrow("Cannot update customer 123");
    })
})