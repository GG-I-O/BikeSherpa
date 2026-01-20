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
import { createRandomCustomer, linkType } from "@/fixtures/customer-fixtures";
import { faker } from "@faker-js/faker";

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
        jest.clearAllMocks();
        mockCustomer1 = createRandomCustomer(true, linkType.updateAndDelete);
        mockCustomer2 = createRandomCustomer(true, linkType.update);
        mockCustomer3 = createRandomCustomer(false, linkType.getAndUpdate);
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
        expect(store[mockCustomer1.id].name).toBe(mockCustomer1.name);
        expect(store[mockCustomer2.id].code).toBe(mockCustomer2.code);
    }, 10000);

    it("should add a customer in the store", async () => {
        //arrange
        backEndClient.AddEndpoint.mockResolvedValue(mockCustomer3.id);
        backEndClient.GetEndpoint.mockResolvedValue({ ...mockCustomer3, createdAt: faker.date.anytime.toString() });

        //act
        customerStore[mockCustomer3.id].set(mockCustomer3);
        let store;
        await when(() => {
            store = customerStore.get();
            return store![mockCustomer3.id].createdAt !== undefined;
        });

        //assert
        expect(store![mockCustomer3.id].name).toBe(mockCustomer3.name);
        expect(store![mockCustomer3.id].code).toBe(mockCustomer3.code);
        expect(store![mockCustomer3.id].createdAt).toBe(mockCustomer3.createdAt);
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.AddEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should update a customer in the store with backend data", async () => {
        //arrange
        await when(() => Object.keys(customerStore.get()).length > 0);

        backEndClient.UpdateEndpoint.mockResolvedValue();
        backEndClient.GetEndpoint.mockResolvedValue({ ...mockCustomer2, name: "Updated Name", updatedAt: "2024-02-02T00:00:00.000Z", createdAt: "2024-02-02T00:00:00.000Z", });


        //act
        customerStore[mockCustomer2.id].assign({
            name: "Updated Name",
            updatedAt: new Date().toISOString()
        });

        await new Promise(resolve => setTimeout(resolve, 500));

        const store = customerStore.get();

        //assert
        expect(store![mockCustomer2.id].name).toBe("Updated Name");
        expect(backEndClient.UpdateEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should delete a customer in the store", async () => {
        //arrange
        await when(() => Object.keys(customerStore.get()).length > 0);
        backEndClient.DeleteEndpoint.mockResolvedValue();

        //act
        customerStore[mockCustomer1.id].delete();

        let store;
        await when(() => {
            store = customerStore.get();
            return store![mockCustomer1.id] === undefined;
        });

        await new Promise(resolve => setTimeout(resolve, 500));

        //assert
        expect(store![mockCustomer1.id]).toBeUndefined();
        expect(backEndClient.DeleteEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should get item using link href when 'self' link exists", async () => {
        //arrange
        const customerWithSelfLink = {
            ...mockCustomer3,
            links: [{
                href: `https://api.example.com/customers/${mockCustomer3.id}`,
                rel: "self",
                method: "GET"
            },
            {
                href: "",
                rel: hateoasRel.update,
                method: "PUT"
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

        backEndClient.AddEndpoint.mockResolvedValue(mockCustomer3.id);
        backEndClient.UpdateEndpoint.mockResolvedValue();

        //act
        customerStore[mockCustomer3.id].set(customerWithSelfLink);

        await when(() => customerStore[mockCustomer3.id].peek() !== undefined);

        customerStore[mockCustomer3.id].assign({
            name: "Trigger Update",
            updatedAt: new Date().toISOString()
        });

        await new Promise(resolve => setTimeout(resolve, 1000));

        //assert
        expect(axios.get).toHaveBeenCalledWith(`https://api.example.com/customers/${mockCustomer3.id}`);
    }, 10000);
})