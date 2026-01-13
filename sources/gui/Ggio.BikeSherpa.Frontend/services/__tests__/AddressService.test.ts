import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IAddressService } from "@/spi/AddressSPI";
import { ILogger } from "@/spi/LogsSPI";
import { Container } from "inversify";
import AddressService from "../AddressService";
import { Address } from "@/models/Address";

global.fetch = jest.fn();

const container = new Container();
const mockLogger: jest.Mocked<ILogger> = {
    error: jest.fn(),
    warn: jest.fn(),
    info: jest.fn(),
    debug: jest.fn(),
    extend: jest.fn()
};
container.bind<ILogger>(ServicesIdentifiers.Logger).toConstantValue(mockLogger);
container.bind<IAddressService>(ServicesIdentifiers.AddressService).to(AddressService);
const addressService = container.get<IAddressService>(ServicesIdentifiers.AddressService);

describe("AddressService.fetchAddress", () => {
    beforeEach(() => {
        (fetch as jest.MockedFunction<typeof fetch>).mockClear();
    });
    it("fetch with a 400 response returns null", async () => {
        // Arrange
        const mockResponse = {
            ok: false,
            status: 400,
            json: async () => ([]),
        };
        (fetch as jest.MockedFunction<typeof fetch>).mockResolvedValueOnce(
            mockResponse as Response
        );

        //Act
        const data = await addressService.fetchAddress("");

        //Assert
        expect(data).toBe(null);
        expect(mockLogger.error).toHaveBeenCalledTimes(1);
    })

    it("fetch with a 200 response returns a one Address array", async () => {
        // Arrange
        const mockResponse = {
            ok: true,
            status: 200,
            json: async () => ({
                features: [{
                    properties: {
                        label: "label 1",
                        name: "name",
                        postcode: "postcode",
                        city: "city"
                    }
                }]
            }),
        };
        (fetch as jest.MockedFunction<typeof fetch>).mockResolvedValueOnce(
            mockResponse as Response
        );

        //Act
        const addressList: Address[] | null = await addressService.fetchAddress("");

        //Assert
        expect(addressList).toBeTruthy();
        expect(Array.isArray(addressList)).toBeTruthy();
        expect(addressList).toHaveLength(1);
        if (!addressList) return
        expect(addressList[0].name).toBe("");
        expect(addressList[0].fullAddress).toBe("label 1");
        expect(addressList[0].streetInfo).toBe("name");
        expect(addressList[0].postcode).toBe("postcode");
        expect(addressList[0].city).toBe("city");
    });

    it("fetch with a 200 response returns a multiple Address array", async () => {
        // Arrange
        const mockResponse = {
            ok: true,
            status: 200,
            json: async () => ({
                features: [
                    {
                        properties: {
                            label: "label 1",
                            name: "name 1",
                            postcode: "postcode 1",
                            city: "city 1"
                        }
                    },
                    {
                        properties: {
                            label: "label 2",
                            name: "name 2",
                            postcode: "postcode 2",
                            city: "city 2"
                        }
                    },
                    {
                        properties: {
                            label: "label 3",
                            name: "name 3",
                            postcode: "postcode 3",
                            city: "city 3"
                        }
                    }
                ]
            }),
        };
        (fetch as jest.MockedFunction<typeof fetch>).mockResolvedValueOnce(
            mockResponse as Response
        );

        //Act
        const data = await addressService.fetchAddress("10 avenue");

        //Assert
        expect(data).toBeTruthy();
        expect(Array.isArray(data)).toBeTruthy();
        expect(data).toHaveLength(3);
        if (!data) return;
        expect(data[1].postcode).toBe("postcode 2");
        expect(data[2].fullAddress).toBe("label 3");
    });

    it("fetch with a 400 response returns a null address list", async () => {
        // Arrange
        const mockResponse = {
            ok: false,
            status: 400,
            json: async () => ({
                features: [{}]
            }),
        };
        (fetch as jest.MockedFunction<typeof fetch>).mockRejectedValue(
            mockResponse as Response
        );

        //Act
        const addressList = await addressService.fetchAddress("");

        //Assert
        expect(addressList).toBeNull();
    });
});
