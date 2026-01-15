import Customer from "@/customers/models/Customer";
import CustomerStorageContext from "@/customers/services/CustomerStorageContext";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService } from "@/spi/StorageSPI";
import { Observable, when } from "@legendapp/state";
import { mock } from "ts-jest-mocker";
import * as Network from 'expo-network';
import { hateoasRel } from "@/models/HateoasLink";
import axios from "axios";

const logger = mock<ILogger>();
const notificationService = mock<INotificationService>();
const backEndClient = mock<IBackendClient<Customer>>();

// Mock expo-network
jest.mock('expo-network');

// Mock axios
jest.mock('axios');

// Disable persistence for tests to avoid rehydrating stale state
jest.mock("@legendapp/state/persist-plugins/mmkv", () => ({
    ObservablePersistMMKV: class {
        getTable = (_: string, initial: any) => initial;
        getMetadata = (_: string, initial: any) => initial;
        getStorage = () => ({ delete: jest.fn(), set: jest.fn() });
        set = jest.fn();
        setMetadata = jest.fn();
        deleteTable = jest.fn();
        deleteMetadata = jest.fn();
        setValue = jest.fn();
        save = jest.fn();
    }
}));

describe("CustomerStorageContext", () => {
    let mockCustomer1: Customer;
    let mockCustomer2: Customer;
    let mockCustomer3: Customer;
    let customerStore: Observable<Record<string, Customer>>;
    beforeEach(() => {
        mockCustomer1 = {
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
            email: "existing.company@gmail.com",
            createdAt: "2024-01-01T00:00:00.000Z",
            updatedAt: "2024-01-01T00:00:00.000Z",
            links: [{
                href: "",
                rel: hateoasRel.update,
                method: ""
            },
            {
                href: "",
                rel: hateoasRel.delete,
                method: ""
            }]
        };

        mockCustomer2 = {
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
            email: "existing.company@gmail.com",
            createdAt: "2024-01-01T00:00:00.000Z",
            updatedAt: "2024-01-01T00:00:00.000Z",
            links: [{
                href: "",
                rel: hateoasRel.update,
                method: ""
            }]
        };
        mockCustomer3 = {
            id: "789",
            name: "Yet Another Existing Company",
            address: {
                name: "Yet Another Existing Company",
                fullAddress: "30 rue de la Paix 75000 Paris",
                streetInfo: "30 rue de la Paix",
                complement: undefined,
                postcode: "75000",
                city: "Paris"
            },
            code: "EX3",
            phoneNumber: "0609080755",
            email: "existing.company@gmail.com",
            links: [{
                href: "https://api.example.com/customers/789",
                rel: hateoasRel.get,
                method: "GET"
            },
            {
                href: "",
                rel: hateoasRel.update,
                method: ""
            }]
        };
        logger.extend.mockReturnValue(logger);
        logger.error = jest.fn();
        logger.info = jest.fn();
        logger.warn = jest.fn();
        logger.debug = jest.fn();

        (Network.getNetworkStateAsync as jest.Mock).mockResolvedValue({
            isInternetReachable: true,
            isConnected: true
        });
        (Network.addNetworkStateListener as jest.Mock).mockReturnValue({ remove: jest.fn() });

        notificationService.start = jest.fn().mockResolvedValue(undefined);
        notificationService.getConnection = jest.fn().mockReturnValue(null);

        backEndClient.GetAllEndpoint.mockResolvedValue([mockCustomer1, mockCustomer2]);
        const customerStorageContext = new CustomerStorageContext(logger, notificationService, backEndClient);
        customerStore = customerStorageContext.getStore();
    })

    afterEach(() => {
        jest.clearAllMocks();
    })

    it("should load customers from backend into the store", async () => {
        //arrange

        //act
        await when(() => Object.keys(customerStore.get()).length > 0);

        const store = customerStore.get();

        //assert
        expect(backEndClient.GetAllEndpoint).toHaveBeenCalledTimes(1);
        expect(store["123"].name).toBe("Existing Company");
        expect(store["456"].code).toBe("EX2");
    }, 10000);

    it("should add a customer in the store", async () => {
        //arrange
        backEndClient.AddEndpoint.mockResolvedValue("789");
        backEndClient.GetEndpoint.mockResolvedValue({ ...mockCustomer3, createdAt: "aupif" });

        //act
        customerStore["789"].set(mockCustomer3);
        let store;
        await when(() => {
            store = customerStore.get();
            return store!["789"].createdAt !== undefined;
        });

        //assert
        expect(store!["789"].name).toBe("Yet Another Existing Company");
        expect(store!["789"].code).toBe("EX3");
        expect(store!["789"].createdAt).toBe("aupif");
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.AddEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should update a customer in the store with backend data", async () => {
        //arrange
        await when(() => Object.keys(customerStore.get()).length > 0);

        backEndClient.UpdateEndpoint.mockResolvedValue();
        backEndClient.GetEndpoint.mockResolvedValue({ ...mockCustomer2, name: "Updated Name", updatedAt: "2024-02-02T00:00:00.000Z", createdAt: "2024-02-02T00:00:00.000Z", });


        //act
        customerStore["456"].assign({
            name: "Updated Name",
            updatedAt: new Date().toISOString()
        });

        await new Promise(resolve => setTimeout(resolve, 500));

        const store = customerStore.get();

        //assert
        expect(store!["456"].name).toBe("Updated Name");
        expect(backEndClient.UpdateEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should delete a customer in the store", async () => {
        //arrange
        await when(() => Object.keys(customerStore.get()).length > 0);
        backEndClient.DeleteEndpoint.mockResolvedValue();

        //act
        customerStore["123"].delete();

        let store;
        await when(() => {
            store = customerStore.get();
            return store!["123"] === undefined;
        });

        await new Promise(resolve => setTimeout(resolve, 500));

        //assert
        expect(store!["123"]).toBeUndefined();
        expect(backEndClient.DeleteEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should get item using link href when 'self' link exists", async () => {
        //arrange
        const customerWithSelfLink = {
            ...mockCustomer3,
            links: [{
                href: "https://api.example.com/customers/789",
                rel: "self",
                method: "GET"
            },
            {
                href: "",
                rel: hateoasRel.update,
                method: ""
            }]
        };

        (axios.get as jest.Mock).mockResolvedValue({
            data: {
                data: {
                    ...mockCustomer3,
                    name: "Fetched via Self Link",
                    code: "SELF1",
                    phoneNumber: "0609080799",
                    email: "self.customer@gmail.com",
                    createdAt: "2024-01-01T00:00:00.000Z",
                    updatedAt: "2024-01-01T00:00:00.000Z"
                },
                links: []
            }
        });

        backEndClient.AddEndpoint.mockResolvedValue("789");
        backEndClient.UpdateEndpoint.mockResolvedValue();

        //act
        customerStore["789"].set(customerWithSelfLink);

        await when(() => customerStore["789"].peek() !== undefined);

        customerStore["789"].assign({
            name: "Trigger Update",
            updatedAt: new Date().toISOString()
        });

        await new Promise(resolve => setTimeout(resolve, 1000));

        //assert
        expect(axios.get).toHaveBeenCalledWith("https://api.example.com/customers/789");
    }, 10000);
})