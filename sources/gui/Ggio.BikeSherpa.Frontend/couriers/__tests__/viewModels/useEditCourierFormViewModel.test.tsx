import { renderHook, act } from "@testing-library/react-native";
import { useEditCourierFormViewModel } from "../../viewModels/useEditCourierFormViewModel";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICourierService } from "@/spi/CourierSPI";
import { Observable, observable } from "@legendapp/state";
import Courier from "@/couriers/models/Courier";
import EditCourierFormViewModel from "@/couriers/viewModels/EditCourierFormViewModel";
import { mock } from "ts-jest-mocker";
import { z } from "zod";

const mockViewModel = mock(EditCourierFormViewModel);

jest.mock('@/bootstrapper/constants/IOCContainer');
jest.mock('../../viewModels/EditCourierFormViewModel', () => {
    return jest.fn(() => mockViewModel);
});

const mockCourierService = mock<ICourierService>();

describe('useEditCourierFormViewModel', () => {
    let mockCourierStore$: any;
    let mockCourier$: Observable<Courier>;
    const mockCourier = mock(Courier);

    beforeEach(() => {
        mockCourierStore$ = observable<Record<string, Courier>>({});
        mockCourier$ = observable<Courier>(mockCourier);
        mockCourierService.getCourierList$.mockReturnValue(mockCourierStore$);
        mockCourierService.getCourier$.mockReturnValue(mockCourier$);

        (IOCContainer.get as jest.Mock).mockReturnValue(mockCourierService);

        // Setup mock view model methods
        mockViewModel.getEditCourierSchema = jest.fn().mockReturnValue(z.object({}));
        mockViewModel.onSubmit = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('properly invokes onSubmit callback when form is submitted', async () => {
        const { result } = renderHook(() => useEditCourierFormViewModel(mockCourier.id));

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

        renderHook(() => useEditCourierFormViewModel(mockCourier.id));

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getEditCourierSchema).toHaveBeenCalledWith(mockCourier, [
            { id: "1", name: "Courier 1" },
            { id: "2", name: "Courier 2" }
        ]);
    });

    it("handles null/undefined observable state", async () => {
        mockCourierStore$.set(null as any);

        renderHook(() => useEditCourierFormViewModel(mockCourier.id));

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getEditCourierSchema).toHaveBeenCalledWith(mockCourier, []);
    });
});