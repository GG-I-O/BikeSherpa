import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { Address } from "@/models/Address";
import { IAddressService } from "@/spi/AddressSPI";
import { ILogger } from "@/spi/LogsSPI";
import { Container } from "inversify";
import AddressService from "../AddressService";
import { createApiClient } from "@/infra/openAPI/client";

jest.mock("@/infra/openAPI/client", () => ({
    createApiClient: jest.fn()
}));

const mockGetAddressSuggestionEndpoint = jest.fn();

const mockApiClient = {
    GetAddressSuggestionEndpoint: mockGetAddressSuggestionEndpoint
};

const mockedCreateApiClient = createApiClient as jest.MockedFunction<
    typeof createApiClient
>;

const container = new Container();

const mockLogger: jest.Mocked<ILogger> = {
    error: jest.fn(),
    warn: jest.fn(),
    info: jest.fn(),
    debug: jest.fn(),
    extend: jest.fn()
};

container
    .bind<ILogger>(ServicesIdentifiers.Logger)
    .toConstantValue(mockLogger);

container
    .bind<IAddressService>(ServicesIdentifiers.AddressService)
    .to(AddressService);

describe("AddressService.fetchAddress", () => {
    let addressService: IAddressService;

    beforeEach(() => {
        jest.clearAllMocks();

        mockedCreateApiClient.mockReturnValue(mockApiClient as any);

        addressService = container.get<IAddressService>(
            ServicesIdentifiers.AddressService
        );
    });

    it("calls the address endpoint with the query", async () => {
        mockGetAddressSuggestionEndpoint.mockResolvedValueOnce([]);

        await addressService.fetchAddress("10 avenue");

        expect(mockGetAddressSuggestionEndpoint).toHaveBeenCalledWith({
            params: {
                query: "10 avenue"
            }
        });
    });

    it("returns null when the address endpoint throws", async () => {
        const error = new Error("Bad request");

        mockGetAddressSuggestionEndpoint.mockRejectedValueOnce(error);

        const data = await addressService.fetchAddress("");

        expect(data).toBeNull();
        expect(mockLogger.error).toHaveBeenCalledTimes(1);
        expect(mockLogger.error).toHaveBeenCalledWith(error);
    });

    it("returns a one Address array", async () => {
        mockGetAddressSuggestionEndpoint.mockResolvedValueOnce([
            {
                name: "",
                streetInfo: "name",
                postcode: "postcode",
                city: "city",
                coordinates: {
                    longitude: "1",
                    latitude: "2"
                }
            }
        ]);

        const addressList = await addressService.fetchAddress("");

        expect(addressList).toBeTruthy();
        expect(addressList).toHaveLength(1);

        if (!addressList) return;

        expect(addressList[0]).toEqual({
            name: "",
            phone: "",
            fullAddress: "name postcode city",
            streetInfo: "name",
            complement: null,
            postcode: "postcode",
            city: "city",
            coordinates: {
                longitude: 1,
                latitude: 2
            }
        });
    });

    it("returns multiple Address objects", async () => {
        mockGetAddressSuggestionEndpoint.mockResolvedValueOnce([
            {
                name: "name 1",
                streetInfo: "street 1",
                postcode: "postcode 1",
                city: "city 1",
                coordinates: {
                    longitude: "1",
                    latitude: "2"
                }
            },
            {
                name: "name 2",
                streetInfo: "street 2",
                postcode: "postcode 2",
                city: "city 2",
                coordinates: {
                    longitude: "3",
                    latitude: "4"
                }
            },
            {
                name: "name 3",
                streetInfo: "street 3",
                postcode: "postcode 3",
                city: "city 3",
                coordinates: {
                    longitude: "5",
                    latitude: "6"
                }
            }
        ]);

        const addressList: Address[] | null =
            await addressService.fetchAddress("10 avenue");

        expect(addressList).toBeTruthy();
        expect(addressList).toHaveLength(3);

        if (!addressList) return;

        expect(addressList[0].name).toBe("name 1");
        expect(addressList[0].fullAddress).toBe(
            "street 1 postcode 1 city 1"
        );
        expect(addressList[0].coordinates).toEqual({
            longitude: 1,
            latitude: 2
        });

        expect(addressList[1].postcode).toBe("postcode 2");
        expect(addressList[1].fullAddress).toBe(
            "street 2 postcode 2 city 2"
        );
        expect(addressList[1].coordinates).toEqual({
            longitude: 3,
            latitude: 4
        });

        expect(addressList[2].fullAddress).toBe(
            "street 3 postcode 3 city 3"
        );
        expect(addressList[2].coordinates).toEqual({
            longitude: 5,
            latitude: 6
        });
    });

    it("converts coordinate strings to numbers", async () => {
        mockGetAddressSuggestionEndpoint.mockResolvedValueOnce([
            {
                name: "name",
                streetInfo: "street",
                postcode: "postcode",
                city: "city",
                coordinates: {
                    longitude: "48.123",
                    latitude: "-2.456"
                }
            }
        ]);

        const addressList = await addressService.fetchAddress("street");

        expect(addressList?.[0].coordinates).toEqual({
            longitude: 48.123,
            latitude: -2.456
        });
    });
});