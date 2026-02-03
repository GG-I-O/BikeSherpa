import { CourierBackendClientFacade } from "@/couriers/services/CourierBackendClientFacade";
import { createRandomCourier, createRandomCourierDto, linkType } from "@/fixtures/courier-fixtures";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import { faker } from "@faker-js/faker";
import axios from "axios";

// Mock the API client module
jest.mock("@/infra/openAPI/client", () => ({
    createApiClient: jest.fn(),
    schemas: {
        CourierCrud: {
            safeParse: jest.fn()
        }
    }
}));

// Mock axios
jest.mock("axios", () => {
    const mockAxios = {
        defaults: {
            baseURL: "https://api.example.com"
        }
    };
    return {
        default: mockAxios,
        __esModule: true
    };
});

describe("CourierBackendClientFacade", () => {
    let facade: CourierBackendClientFacade;
    let mockApiClient: any;

    beforeEach(() => {
        // Reset mocks
        jest.clearAllMocks();

        // Create mock API client
        mockApiClient = {
            GetAllCouriersEndpoint: jest.fn(),
            GetCourierEndpoint: jest.fn(),
            AddCourierEndpoint: jest.fn(),
            UpdateCourierEndpoint: jest.fn(),
            DeleteCourierEndpoint: jest.fn()
        };

        // Mock createApiClient to return our mock
        (createApiClient as jest.Mock).mockReturnValue(mockApiClient);

        // Create facade instance
        facade = new CourierBackendClientFacade();
    });

    describe("Constructor", () => {
        it("should handle missing baseURL gracefully", () => {
            // Arrange
            const originalBaseURL = axios.defaults.baseURL;
            (axios.defaults as any).baseURL = undefined;

            // Reset and spy on createApiClient
            (createApiClient as jest.Mock).mockClear();

            // Act
            new CourierBackendClientFacade();

            // Assert
            expect(createApiClient).toHaveBeenCalledWith('', { axiosInstance: axios });

            // Restore
            axios.defaults.baseURL = originalBaseURL;
        });
    });

    describe("GetAllEndpoint", () => {
        it("should fetch all couriers without lastSync parameter", async () => {
            // Arrange
            const mockCourierDtos = faker.helpers.multiple(() => createRandomCourierDto(), {
                count: 3
            });
            mockApiClient.GetAllCouriersEndpoint.mockResolvedValue(mockCourierDtos);

            // Act
            const result = await facade.GetAllEndpoint();

            // Assert
            expect(mockApiClient.GetAllCouriersEndpoint).toHaveBeenCalledWith({
                params: { lastSync: '' }
            });
            expect(result).toHaveLength(3);
            expect(result[0].firstName).toBe(mockCourierDtos[0].data.firstName);
            expect(result[0].id).toBe(mockCourierDtos[0].data.id);
        });

        it("should fetch all couriers with lastSync parameter", async () => {
            // Arrange
            const lastSync = "2026-01-01T00:00:00Z";
            const mockCourierDtos = faker.helpers.multiple(() => createRandomCourierDto(), {
                count: 2
            });
            mockApiClient.GetAllCouriersEndpoint.mockResolvedValue(mockCourierDtos);

            // Act
            const result = await facade.GetAllEndpoint(lastSync);

            // Assert
            expect(mockApiClient.GetAllCouriersEndpoint).toHaveBeenCalledWith({
                params: { lastSync }
            });
            expect(result).toHaveLength(2);
        });

        it("should return empty array when no couriers exist", async () => {
            // Arrange
            mockApiClient.GetAllCouriersEndpoint.mockResolvedValue([]);

            // Act
            const result = await facade.GetAllEndpoint();

            // Assert
            expect(result).toEqual([]);
        });
    });

    describe("GetEndpoint", () => {
        it("should fetch a single courier by id", async () => {
            // Arrange
            const mockCourierDto = createRandomCourierDto();
            const courierId = mockCourierDto.data.id;
            mockApiClient.GetCourierEndpoint.mockResolvedValue(mockCourierDto);

            // Act
            const result = await facade.GetEndpoint(courierId);

            // Assert
            expect(mockApiClient.GetCourierEndpoint).toHaveBeenCalledWith({
                params: { courierId }
            });
            expect(result).not.toBeNull();
            expect(result?.id).toBe(courierId);
            expect(result?.firstName).toBe(mockCourierDto.data.firstName);
        });
    });

    describe("AddEndpoint", () => {
        it("should create a courier successfully", async () => {
            // Arrange
            const courier = createRandomCourier(false, linkType.none);
            const newId = faker.string.uuid();

            // Mock schema validation success
            (schemas.CourierCrud.safeParse as jest.Mock).mockReturnValue({
                success: true,
                data: courier
            });

            mockApiClient.AddCourierEndpoint.mockResolvedValue({ id: newId });

            // Act
            const result = await facade.AddEndpoint(courier);

            // Assert
            expect(schemas.CourierCrud.safeParse).toHaveBeenCalledWith(courier);
            expect(mockApiClient.AddCourierEndpoint).toHaveBeenCalledWith(
                courier,
                {
                    headers: { operationId: courier.operationId }
                }
            );
            expect(result).toBe(newId);
        });

        it("should set complement to empty string when undefined", async () => {
            // Arrange
            const courier = createRandomCourier(false, linkType.none);
            courier.address.complement = undefined;
            const newId = faker.string.uuid();

            (schemas.CourierCrud.safeParse as jest.Mock).mockReturnValue({
                success: true,
                data: courier
            });

            mockApiClient.AddCourierEndpoint.mockResolvedValue({ id: newId });

            // Act
            await facade.AddEndpoint(courier);

            // Assert
            expect(courier.address.complement).toBe("");
        });

        it("should throw error when validation fails", async () => {
            // Arrange
            const courier = createRandomCourier(false, linkType.none);
            const validationError = {
                format: jest.fn().mockReturnValue({ errors: "validation failed" })
            };

            (schemas.CourierCrud.safeParse as jest.Mock).mockReturnValue({
                success: false,
                error: validationError
            });

            // Mock console.error to suppress expected error output
            const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation();

            // Act & Assert
            await expect(facade.AddEndpoint(courier)).rejects.toEqual(validationError);

            // Restore console.error
            consoleErrorSpy.mockRestore();
        });
    });

    describe("UpdateEndpoint", () => {
        it("should update a courier successfully", async () => {
            // Arrange
            const courier = createRandomCourier(true, linkType.update);

            (schemas.CourierCrud.safeParse as jest.Mock).mockReturnValue({
                success: true,
                data: courier
            });

            mockApiClient.UpdateCourierEndpoint.mockResolvedValue(undefined);

            // Act
            await facade.UpdateEndpoint(courier);

            // Assert
            expect(schemas.CourierCrud.safeParse).toHaveBeenCalledWith(courier);
            expect(mockApiClient.UpdateCourierEndpoint).toHaveBeenCalledWith(
                courier,
                {
                    params: { courierId: courier.id },
                    headers: { operationId: courier.operationId }
                }
            );
        });

        it("should set complement to empty string when undefined", async () => {
            // Arrange
            const courier = createRandomCourier(true, linkType.update);
            courier.address.complement = undefined;

            (schemas.CourierCrud.safeParse as jest.Mock).mockReturnValue({
                success: true,
                data: courier
            });

            mockApiClient.UpdateCourierEndpoint.mockResolvedValue(undefined);

            // Act
            await facade.UpdateEndpoint(courier);

            // Assert
            expect(courier.address.complement).toBe("");
        });

        it("should throw error when validation fails", async () => {
            // Arrange
            const courier = createRandomCourier(true, linkType.update);
            const validationError = {
                format: jest.fn().mockReturnValue({ errors: "validation failed" })
            };

            (schemas.CourierCrud.safeParse as jest.Mock).mockReturnValue({
                success: false,
                error: validationError
            });

            // Mock console.error to suppress expected error output
            const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation();

            // Act & Assert
            await expect(facade.UpdateEndpoint(courier)).rejects.toEqual(validationError);

            // Restore console.error
            consoleErrorSpy.mockRestore();
        });
    });

    describe("DeleteEndpoint", () => {
        it("should delete a courier successfully", async () => {
            // Arrange
            const courier = createRandomCourier(true, linkType.delete);
            mockApiClient.DeleteCourierEndpoint.mockResolvedValue(undefined);

            // Act
            await facade.DeleteEndpoint(courier);

            // Assert
            expect(mockApiClient.DeleteCourierEndpoint).toHaveBeenCalledWith(
                undefined,
                {
                    params: { courierId: courier.id },
                    headers: { operationId: courier.operationId }
                }
            );
        });
    });
});
