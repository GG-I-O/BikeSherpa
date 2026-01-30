import Courier from "@/couriers/models/Courier";
import CourierStorageContext from "@/couriers/services/CourierStorageContext";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService } from "@/spi/StorageSPI";
import { Observable, when } from "@legendapp/state";
import { mock } from "ts-jest-mocker";
import * as Network from "expo-network";
import { hateoasRel } from "@/models/HateoasLink";
import axios from "axios";
import { createRandomCourier, linkType } from "@/fixtures/courier-fixtures";
import { faker } from "@faker-js/faker";

const logger = mock<ILogger>();
const notificationService = mock<INotificationService>();
const backEndClient = mock<IBackendClient<Courier>>();

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

describe("CourierStorageContext", () => {
    let mockCourier1: Courier;
    let mockCourier2: Courier;
    let mockCourier3: Courier;
    let courierStore: Observable<Record<string, Courier>>;
    beforeEach(() => {
        jest.clearAllMocks();
        mockCourier1 = createRandomCourier(true, linkType.updateAndDelete);
        mockCourier2 = createRandomCourier(true, linkType.update);
        mockCourier3 = createRandomCourier(false, linkType.getAndUpdate);
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

        backEndClient.GetAllEndpoint.mockResolvedValue([mockCourier1, mockCourier2]);
        const courierStorageContext = new CourierStorageContext(logger, notificationService, backEndClient);
        courierStore = courierStorageContext.getStore();
    })

    afterEach(() => {
        jest.clearAllMocks();
    })

    it("should load couriers from backend into the store", async () => {
        //arrange

        //act
        await when(() => Object.keys(courierStore.get()).length > 0);

        const store = courierStore.get();

        //assert
        expect(backEndClient.GetAllEndpoint).toHaveBeenCalledTimes(1);
        expect(store[mockCourier1.id].firstName).toBe(mockCourier1.firstName);
        expect(store[mockCourier2.id].code).toBe(mockCourier2.code);
    }, 10000);

    it("should add a courier in the store", async () => {
        //arrange
        backEndClient.AddEndpoint.mockResolvedValue(mockCourier3.id);
        backEndClient.GetEndpoint.mockResolvedValue({ ...mockCourier3, createdAt: faker.date.anytime.toString() });

        //act
        courierStore[mockCourier3.id].set(mockCourier3);
        let store;
        await when(() => {
            store = courierStore.get();
            return store![mockCourier3.id].createdAt !== undefined;
        });

        //assert
        expect(store![mockCourier3.id].firstName).toBe(mockCourier3.firstName);
        expect(store![mockCourier3.id].code).toBe(mockCourier3.code);
        expect(store![mockCourier3.id].createdAt).toBe(mockCourier3.createdAt);
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.AddEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should update a courier in the store with backend data", async () => {
        //arrange
        await when(() => Object.keys(courierStore.get()).length > 0);

        backEndClient.UpdateEndpoint.mockResolvedValue();
        backEndClient.GetEndpoint.mockResolvedValue({ ...mockCourier2, lastName: "Updated Last Name", updatedAt: "2024-02-02T00:00:00.000Z", createdAt: "2024-02-02T00:00:00.000Z", });


        //act
        courierStore[mockCourier2.id].assign({
            lastName: "Updated Last Name",
            updatedAt: new Date().toISOString()
        });

        await new Promise(resolve => setTimeout(resolve, 500));

        const store = courierStore.get();

        //assert
        expect(store![mockCourier2.id].lastName).toBe("Updated Last Name");
        expect(backEndClient.UpdateEndpoint).toHaveBeenCalledTimes(1);
        expect(backEndClient.GetEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should delete a courier in the store", async () => {
        //arrange
        await when(() => Object.keys(courierStore.get()).length > 0);
        backEndClient.DeleteEndpoint.mockResolvedValue();

        //act
        courierStore[mockCourier1.id].delete();

        let store;
        await when(() => {
            store = courierStore.get();
            return store![mockCourier1.id] === undefined;
        });

        await new Promise(resolve => setTimeout(resolve, 500));

        //assert
        expect(store![mockCourier1.id]).toBeUndefined();
        expect(backEndClient.DeleteEndpoint).toHaveBeenCalledTimes(1);
    }, 10000);

    it("should get item using link href when 'self' link exists", async () => {
        //arrange
        const courierWithSelfLink = {
            ...mockCourier3,
            links: [{
                href: `https://api.example.com/couriers/${mockCourier3.id}`,
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
                    ...mockCourier3,
                    firstName: "Fetched via Self Link",
                    lastName: "SELFLASTNAME1",
                    code: "SELF1",
                    phoneNumber: "0609080799",
                    email: "self.courier@gmail.com",
                    createdAt: "2024-01-01T00:00:00.000Z",
                    updatedAt: "2024-01-01T00:00:00.000Z"
                },
                links: []
            }
        });

        backEndClient.AddEndpoint.mockResolvedValue(mockCourier3.id);
        backEndClient.UpdateEndpoint.mockResolvedValue();

        //act
        courierStore[mockCourier3.id].set(courierWithSelfLink);

        await when(() => courierStore[mockCourier3.id].peek() !== undefined);

        courierStore[mockCourier3.id].assign({
            firstName: "Trigger Update",
            updatedAt: new Date().toISOString()
        });

        await new Promise(resolve => setTimeout(resolve, 1000));

        //assert
        expect(axios.get).toHaveBeenCalledWith(`https://api.example.com/couriers/${mockCourier3.id}`);
    }, 10000);
})