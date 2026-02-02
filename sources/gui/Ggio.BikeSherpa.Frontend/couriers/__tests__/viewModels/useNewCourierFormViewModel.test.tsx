import { renderHook, act } from "@testing-library/react-native";
import { useNewCourierFormViewModel } from "../../viewModels/useNewCourierFormViewModel";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICourierService } from "@/spi/CourierSPI";
import { observable } from "@legendapp/state";
import Courier from "@/couriers/models/Courier";
import NewCourierFormViewModel from "../../viewModels/NewCourierFormViewModel";
import { mock } from "ts-jest-mocker";
import { z } from "zod";

const mockViewModel = mock(NewCourierFormViewModel);

jest.mock("@/bootstrapper/constants/IOCContainer");
jest.mock("../../viewModels/NewCourierFormViewModel", () => {
    return jest.fn(() => mockViewModel);
});

const mockCourierService = mock<ICourierService>();

describe("useNewCourierFormViewModel", () => {
    let mockCourierStore$: any;

    beforeEach(() => {
        mockCourierStore$ = observable<Record<string, Courier>>({});
        mockCourierService.getCourierList$.mockReturnValue(mockCourierStore$);

        (IOCContainer.get as jest.Mock).mockReturnValue(mockCourierService);

        // Setup mock view model methods
        mockViewModel.getNewCourierSchema = jest.fn().mockReturnValue(z.object({}));
        mockViewModel.setResetCallback = jest.fn();
        mockViewModel.onSubmit = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it("properly invokes onSubmit callback when form is submitted", async () => {
        const { result } = renderHook(() => useNewCourierFormViewModel());

        await act(async () => {
            await result.current.handleSubmit({ preventDefault: jest.fn() } as any);
        });

        expect(mockViewModel.onSubmit).toHaveBeenCalled();
    });

    it("updates courierList when observable state changes", async () => {
        mockCourierStore$.set({
            "1": { id: "1", name: "Courier 1" },
            "2": { id: "2", name: "Courier 2" }
        });

        renderHook(() => useNewCourierFormViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getNewCourierSchema).toHaveBeenCalledWith([
            { id: "1", name: "Courier 1" },
            { id: "2", name: "Courier 2" }
        ]);
    });

    it("handles null/undefined observable state", async () => {
        mockCourierStore$.set(null as any);

        renderHook(() => useNewCourierFormViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getNewCourierSchema).toHaveBeenCalledWith([]);
    });
});