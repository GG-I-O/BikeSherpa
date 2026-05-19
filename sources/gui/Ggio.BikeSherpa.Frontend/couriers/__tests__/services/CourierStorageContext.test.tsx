import Courier, { CourierDto } from "@/couriers/models/Courier";
import CourierMapper from "@/couriers/services/CourierMapper";
import CourierStorageContext from "@/couriers/services/CourierStorageContext";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService } from "@/spi/StorageSPI";
import * as Network from "expo-network";
import axios from "axios";
import { createRandomCourier, linkType } from "@/fixtures/courier-fixtures";

jest.mock("expo-network");
jest.mock("axios");

jest.mock("@/infra/openAPI/client", () => ({
    getTagByAlias: jest.fn(() => "Courier")
}));

jest.mock("@legendapp/state/persist-plugins/mmkv", () => ({
    ObservablePersistMMKV: class {
        getTable = (_: string, initial: unknown) => initial;
        getMetadata = (_: string, initial: unknown) => initial;
        getStorage = () => ({
            delete: jest.fn(),
            set: jest.fn()
        });
        set = jest.fn();
        setMetadata = jest.fn();
        deleteTable = jest.fn();
        deleteMetadata = jest.fn();
        setValue = jest.fn();
        save = jest.fn();
    }
}));

type CourierStorageContextTestApi = {
    getList(lastSync?: string): Promise<Courier[]>;
    getItem(id: string): Promise<Courier | null>;
    create(item: Courier): Promise<string>;
    update(item: Courier): Promise<void>;
    delete(item: Courier): Promise<void>;
};

describe("CourierStorageContext", () => {
    let logger: jest.Mocked<ILogger>;
    let notificationService: jest.Mocked<INotificationService>;
    let backendClient: jest.Mocked<IBackendClient<Courier>>;
    let courierStorageContext: CourierStorageContext;
    let courierStorageContextTestApi: CourierStorageContextTestApi;

    beforeEach(() => {
        jest.clearAllMocks();

        logger = {
            extend: jest.fn(),
            error: jest.fn(),
            info: jest.fn(),
            warn: jest.fn(),
            debug: jest.fn()
        } as unknown as jest.Mocked<ILogger>;

        logger.extend.mockReturnValue(logger);

        notificationService = {
            start: jest.fn().mockResolvedValue(undefined),
            getConnection: jest.fn().mockReturnValue(null),
            onReconnected: jest.fn()
        } as unknown as jest.Mocked<INotificationService>;

        backendClient = {
            GetAllEndpoint: jest.fn(),
            GetEndpoint: jest.fn(),
            AddEndpoint: jest.fn(),
            UpdateEndpoint: jest.fn(),
            DeleteEndpoint: jest.fn()
        } as unknown as jest.Mocked<IBackendClient<Courier>>;

        (Network.getNetworkStateAsync as jest.Mock).mockResolvedValue({
            isInternetReachable: false,
            isConnected: false
        });

        (Network.addNetworkStateListener as jest.Mock).mockReturnValue({
            remove: jest.fn()
        });

        courierStorageContext = new CourierStorageContext(
            logger,
            notificationService,
            backendClient
        );

        courierStorageContextTestApi =
            courierStorageContext as unknown as CourierStorageContextTestApi;
    });

    it("creates an observable store", () => {
        const store = courierStorageContext.getStore();

        expect(store).toBeDefined();
        expect(typeof store.get).toBe("function");
        expect(typeof store.peek).toBe("function");
    });

    it("loads couriers from the backend client", async () => {
        const courier1 = createRandomCourier(true, linkType.updateAndDelete);
        const courier2 = createRandomCourier(true, linkType.update);

        backendClient.GetAllEndpoint.mockResolvedValue([courier1, courier2]);

        const result = await courierStorageContextTestApi.getList();

        expect(backendClient.GetAllEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.GetAllEndpoint).toHaveBeenCalledWith(undefined);
        expect(result).toEqual([courier1, courier2]);
    });

    it("loads couriers from the backend client with a lastSync date", async () => {
        const lastSync = "2026-05-19T10:00:00.000Z";
        const courier = createRandomCourier(true, linkType.updateAndDelete);

        backendClient.GetAllEndpoint.mockResolvedValue([courier]);

        const result = await courierStorageContextTestApi.getList(lastSync);

        expect(backendClient.GetAllEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.GetAllEndpoint).toHaveBeenCalledWith(lastSync);
        expect(result).toEqual([courier]);
    });

    it("creates a courier through the backend client", async () => {
        const courier = createRandomCourier(false, linkType.getAndUpdate);

        backendClient.AddEndpoint.mockResolvedValue(courier.id);

        const result = await courierStorageContextTestApi.create(courier);

        expect(backendClient.AddEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.AddEndpoint).toHaveBeenCalledWith(courier);
        expect(result).toBe(courier.id);
    });

    it("updates a courier through the backend client", async () => {
        const courier = createRandomCourier(true, linkType.updateAndDelete);

        backendClient.UpdateEndpoint.mockResolvedValue(undefined);

        await courierStorageContextTestApi.update(courier);

        expect(backendClient.UpdateEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.UpdateEndpoint).toHaveBeenCalledWith(courier);
    });

    it("deletes a courier through the backend client", async () => {
        const courier = createRandomCourier(true, linkType.updateAndDelete);

        backendClient.DeleteEndpoint.mockResolvedValue(undefined);

        await courierStorageContextTestApi.delete(courier);

        expect(backendClient.DeleteEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.DeleteEndpoint).toHaveBeenCalledWith(courier);
    });

    it("gets a courier from the backend client when no self link exists in the store", async () => {
        const courier = createRandomCourier(true, linkType.update);

        backendClient.GetEndpoint.mockResolvedValue(courier);

        const result = await courierStorageContextTestApi.getItem(courier.id);

        expect(backendClient.GetEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.GetEndpoint).toHaveBeenCalledWith(courier.id);
        expect(axios.get).not.toHaveBeenCalled();
        expect(result).toBe(courier);
    });

    it("gets a courier with axios and maps it when a self link exists in the store", async () => {
        const selfHref = "https://api.example.test/couriers/123";
        const courier = createRandomCourier(true, linkType.updateAndDelete);

        courier.links = [
            {
                rel: "self",
                href: selfHref,
                method: "GET"
            }
        ];

        courierStorageContext.getStore()[courier.id].set(courier);

        const courierDto: CourierDto = {
            data: {
                id: courier.id,
                firstName: courier.firstName,
                lastName: courier.lastName,
                code: courier.code,
                phoneNumber: courier.phoneNumber,
                email: courier.email,
                address: courier.address,
                createdAt: courier.createdAt ?? "2026-05-19T10:00:00.000Z",
                updatedAt: courier.updatedAt ?? "2026-05-19T10:00:00.000Z"
            },
            links: courier.links
        };

        const mappedCourier = {
            ...courier,
            lastName: "Mapped Last Name"
        };

        (axios.get as jest.Mock).mockResolvedValue({
            data: courierDto
        });

        const mapperSpy = jest
            .spyOn(CourierMapper, "CourierDtoToCourier")
            .mockReturnValue(mappedCourier);

        const result = await courierStorageContextTestApi.getItem(courier.id);

        expect(axios.get).toHaveBeenCalledTimes(1);
        expect(axios.get).toHaveBeenCalledWith(selfHref);

        expect(mapperSpy).toHaveBeenCalledTimes(1);
        expect(mapperSpy).toHaveBeenCalledWith(courierDto);

        expect(backendClient.GetEndpoint).not.toHaveBeenCalled();
        expect(result).toBe(mappedCourier);

        mapperSpy.mockRestore();
    });
});