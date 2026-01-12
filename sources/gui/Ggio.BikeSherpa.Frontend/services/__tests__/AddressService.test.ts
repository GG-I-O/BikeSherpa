import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IAddressService } from "@/spi/AddressSPI";
import { ILogger } from "@/spi/LogsSPI";
import { Container } from "inversify";
import AddressService from "../AddressService";
import { Address } from "@/models/Address";

global.fetch = jest.fn();

const mockAddress1: Address = {
    name: "Société 1",
    fullAddress: "10 rue de la Société 1 Grenoble",
    streetInfo: "10 rue de la Société 1",
    postcode: "38000",
    city: "Grenoble"
}
const mockAddress2: Address = {
    name: "Société 2",
    fullAddress: "10 rue de la Société 2 Grenoble",
    streetInfo: "10 rue de la Société 2",
    postcode: "38300",
    city: "Grenoble"
}
const mockAddress3: Address = {
    name: "Société 3",
    fullAddress: "10 rue de la Société 3 Grenoble",
    streetInfo: "10 rue de la Société 3",
    postcode: "38100",
    city: "Échirolles"
}

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
                        label: mockAddress1.name,
                        name: mockAddress1.streetInfo,
                        postcode: mockAddress1.postcode,
                        city: mockAddress1.city
                    }
                }]
            }),
        };
        (fetch as jest.MockedFunction<typeof fetch>).mockResolvedValueOnce(
            mockResponse as Response
        );

        //Act
        const data = await addressService.fetchAddress("");

        //Assert
        expect(data).toBeTruthy();
        expect(Array.isArray(data)).toBeTruthy();
        expect(data).toHaveLength(1);
        if (!data) return
        expect(data[0].name).toBe("Société 1");
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
                            label: mockAddress1.name,
                            name: mockAddress1.streetInfo,
                            postcode: mockAddress1.postcode,
                            city: mockAddress1.city
                        }
                    },
                    {
                        properties: {
                            label: mockAddress2.name,
                            name: mockAddress2.streetInfo,
                            postcode: mockAddress2.postcode,
                            city: mockAddress2.city
                        }
                    },
                    {
                        properties: {
                            label: mockAddress3.name,
                            name: mockAddress3.streetInfo,
                            postcode: mockAddress3.postcode,
                            city: mockAddress3.city
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
        expect(data[1].name).toBe("Société 2");
        expect(data[2].city).toBe("Échirolles");
    });
});
