import Customer from "@/customers/models/Customer";
import CustomerStorageContext from "@/customers/services/CustomerStorageContext";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService } from "@/spi/StorageSPI";
import { Observable, when } from "@legendapp/state";
import { mock } from "ts-jest-mocker";
import * as Network from 'expo-network';
import { hateoasRel } from "@/models/HateoasLink";

const logger = mock<ILogger>();
const notificationService = mock<INotificationService>();
const backEndClient = mock<IBackendClient<Customer>>();

// Mock expo-network
jest.mock('expo-network');

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
            email: "existing.company@gmail.com"
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
            email: "existing.company@gmail.com"
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

        const updatedCustomer = { ...mockCustomer2, name: "New Name" };
        backEndClient.UpdateEndpoint.mockResolvedValue();
        backEndClient.GetEndpoint.mockResolvedValue({ ...updatedCustomer, createdAt: "blabla" });

        //act
        customerStore["456"].assign(updatedCustomer);

        let store;
        await when(() => {
            store = customerStore.get();
            return store!["456"]?.createdAt !== undefined;
        });

        //assert
        expect(store!["456"].name).toBe("New Name");
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.UpdateEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);
})