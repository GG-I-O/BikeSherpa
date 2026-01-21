import AppSnackbarService from "../../services/AppSnackbarService";
import { IStorageContext } from "@/spi/StorageSPI";
import Customer from "@/customers/models/Customer";
import { EventRegister } from "react-native-event-listeners";

jest.mock("react-native-event-listeners");

describe("AppSnackbarService", () => {
    let mockCustomerStorage: jest.Mocked<IStorageContext<Customer>>;
    let service: AppSnackbarService;
    let onErrorCallback: ((data: string) => void) | undefined;

    beforeEach(() => {
        jest.clearAllMocks();

        mockCustomerStorage = {
            subscribeToOnErrorEvent: jest.fn((callback) => {
                onErrorCallback = callback;
            }),
        } as any;

        service = new AppSnackbarService(mockCustomerStorage);
    });

    describe("Constructor", () => {
        it("subscribes to customer storage error event", () => {
            expect(mockCustomerStorage.subscribeToOnErrorEvent).toHaveBeenCalledTimes(1);
            expect(mockCustomerStorage.subscribeToOnErrorEvent).toHaveBeenCalledWith(expect.any(Function));
        });

        it("emits event when customer storage error occurs", () => {
            const errorMessage = "Une erreur est survenue";

            onErrorCallback?.(errorMessage);

            expect(EventRegister.emit).toHaveBeenCalledWith("snackbar", errorMessage);
        });
    });

    describe("subscribe", () => {
        it("delegates to EventRegister and returns event ID", () => {
            const mockEventId = "test-event-id-123";
            const mockCallback = jest.fn();
            (EventRegister.addEventListener as jest.Mock).mockReturnValue(mockEventId);

            const result = service.subscribe(mockCallback);

            expect(EventRegister.addEventListener).toHaveBeenCalledWith("snackbar", mockCallback);
            expect(result).toBe(mockEventId);
        });

        it("returns empty string when EventRegister returns null", () => {
            const mockCallback = jest.fn();
            (EventRegister.addEventListener as jest.Mock).mockReturnValue(null);

            const result = service.subscribe(mockCallback);

            expect(result).toBe("");
        });

        it("returns empty string when EventRegister returns undefined", () => {
            const mockCallback = jest.fn();
            (EventRegister.addEventListener as jest.Mock).mockReturnValue(undefined);

            const result = service.subscribe(mockCallback);

            expect(result).toBe("");
        });
    });

    describe("unSubscribe", () => {
        it("delegates to EventRegister with correct ID", () => {
            const eventId = "test-event-id-456";

            service.unSubscribe(eventId);

            expect(EventRegister.removeEventListener).toHaveBeenCalledWith(eventId);
        });
    });
});
