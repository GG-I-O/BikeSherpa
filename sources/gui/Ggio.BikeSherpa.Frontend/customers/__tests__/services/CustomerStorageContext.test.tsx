import Customer, { CustomerDto } from "@/customers/models/Customer";
import CustomerMapper from "@/customers/services/CustomerMapper";
import CustomerStorageContext from "@/customers/services/CustomerStorageContext";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService } from "@/spi/StorageSPI";
import * as Network from "expo-network";
import axios from "axios";
import { createRandomCustomer, linkType } from "@/fixtures/customer-fixtures";

jest.mock("expo-network");
jest.mock("axios");

jest.mock("@/infra/openAPI/client", () => ({
    getTagByAlias: jest.fn(() => "Customer")
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

type CustomerStorageContextTestApi = {
    getList(lastSync?: string): Promise<Customer[]>;
    getItem(id: string): Promise<Customer | null>;
    create(item: Customer): Promise<string>;
    update(item: Customer): Promise<void>;
    delete(item: Customer): Promise<void>;
};

describe("CustomerStorageContext", () => {
    let logger: jest.Mocked<ILogger>;
    let notificationService: jest.Mocked<INotificationService>;
    let backendClient: jest.Mocked<IBackendClient<Customer>>;
    let customerStorageContext: CustomerStorageContext;
    let customerStorageContextTestApi: CustomerStorageContextTestApi;

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
        } as unknown as jest.Mocked<IBackendClient<Customer>>;

        (Network.getNetworkStateAsync as jest.Mock).mockResolvedValue({
            isInternetReachable: false,
            isConnected: false
        });

        (Network.addNetworkStateListener as jest.Mock).mockReturnValue({
            remove: jest.fn()
        });

        customerStorageContext = new CustomerStorageContext(
            logger,
            notificationService,
            backendClient
        );

        customerStorageContextTestApi =
            customerStorageContext as unknown as CustomerStorageContextTestApi;
    });

    it("creates an observable store", () => {
        const store = customerStorageContext.getStore();

        expect(store).toBeDefined();
        expect(typeof store.get).toBe("function");
        expect(typeof store.peek).toBe("function");
    });

    it("loads customers from the backend client", async () => {
        const customer1 = createRandomCustomer(true, linkType.updateAndDelete);
        const customer2 = createRandomCustomer(true, linkType.update);

        backendClient.GetAllEndpoint.mockResolvedValue([customer1, customer2]);

        const result = await customerStorageContextTestApi.getList();

        expect(backendClient.GetAllEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.GetAllEndpoint).toHaveBeenCalledWith(undefined);
        expect(result).toEqual([customer1, customer2]);
    });

    it("loads customers from the backend client with a lastSync date", async () => {
        const lastSync = "2026-05-19T10:00:00.000Z";
        const customer = createRandomCustomer(true, linkType.updateAndDelete);

        backendClient.GetAllEndpoint.mockResolvedValue([customer]);

        const result = await customerStorageContextTestApi.getList(lastSync);

        expect(backendClient.GetAllEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.GetAllEndpoint).toHaveBeenCalledWith(lastSync);
        expect(result).toEqual([customer]);
    });

    it("creates a customer through the backend client", async () => {
        const customer = createRandomCustomer(false, linkType.getAndUpdate);

        backendClient.AddEndpoint.mockResolvedValue(customer.id);

        const result = await customerStorageContextTestApi.create(customer);

        expect(backendClient.AddEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.AddEndpoint).toHaveBeenCalledWith(customer);
        expect(result).toBe(customer.id);
    });

    it("updates a customer through the backend client", async () => {
        const customer = createRandomCustomer(true, linkType.updateAndDelete);

        backendClient.UpdateEndpoint.mockResolvedValue(undefined);

        await customerStorageContextTestApi.update(customer);

        expect(backendClient.UpdateEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.UpdateEndpoint).toHaveBeenCalledWith(customer);
    });

    it("deletes a customer through the backend client", async () => {
        const customer = createRandomCustomer(true, linkType.updateAndDelete);

        backendClient.DeleteEndpoint.mockResolvedValue(undefined);

        await customerStorageContextTestApi.delete(customer);

        expect(backendClient.DeleteEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.DeleteEndpoint).toHaveBeenCalledWith(customer);
    });

    it("gets a customer from the backend client when no self link exists in the store", async () => {
        const customer = createRandomCustomer(true, linkType.update);

        backendClient.GetEndpoint.mockResolvedValue(customer);

        const result = await customerStorageContextTestApi.getItem(customer.id);

        expect(backendClient.GetEndpoint).toHaveBeenCalledTimes(1);
        expect(backendClient.GetEndpoint).toHaveBeenCalledWith(customer.id);
        expect(axios.get).not.toHaveBeenCalled();
        expect(result).toBe(customer);
    });

    it("gets a customer with axios and maps it when a self link exists in the store", async () => {
        const selfHref = "https://api.example.test/customers/123";
        const customer = createRandomCustomer(true, linkType.updateAndDelete);

        customer.links = [
            {
                rel: "self",
                href: selfHref,
                method: "GET"
            }
        ];

        customerStorageContext.getStore()[customer.id].set(customer);

        const customerDto: CustomerDto = {
            data: {
                id: customer.id,
                name: customer.name,
                address: customer.address,
                code: customer.code,
                phoneNumber: customer.phoneNumber,
                email: customer.email,
                siret: customer.siret ?? null,
                vatNumber: customer.vatNumber ?? null,
                createdAt: customer.createdAt ?? "2026-05-19T10:00:00.000Z",
                updatedAt: customer.updatedAt ?? "2026-05-19T10:00:00.000Z"
            },
            links: customer.links
        };

        const mappedCustomer = {
            ...customer,
            name: "Mapped Customer"
        };

        (axios.get as jest.Mock).mockResolvedValue({
            data: customerDto
        });

        const mapperSpy = jest
            .spyOn(CustomerMapper, "CustomerDtoToCustomer")
            .mockReturnValue(mappedCustomer);

        const result = await customerStorageContextTestApi.getItem(customer.id);

        expect(axios.get).toHaveBeenCalledTimes(1);
        expect(axios.get).toHaveBeenCalledWith(selfHref);

        expect(mapperSpy).toHaveBeenCalledTimes(1);
        expect(mapperSpy).toHaveBeenCalledWith(customerDto);

        expect(backendClient.GetEndpoint).not.toHaveBeenCalled();
        expect(result).toBe(mappedCustomer);

        mapperSpy.mockRestore();
    });
});