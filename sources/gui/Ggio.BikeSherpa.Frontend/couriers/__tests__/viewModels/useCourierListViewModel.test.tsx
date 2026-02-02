import useCourierListViewModel from "@/couriers/viewModels/useCourierListViewModel";
import { observable } from "@legendapp/state";
import { act, renderHook } from "@testing-library/react-native";
import { navigate } from "expo-router/build/global-state/routing";

jest.mock("expo-router/build/global-state/routing", () => ({
    navigate: jest.fn()
}));

const mockNavigate = navigate as jest.Mock;
const mockCourierStore$ = observable({});
const mockCourierService = {
    getCourierList$: jest.fn(() => mockCourierStore$),
    deleteCourier: jest.fn()
};
const mockDeleteCourier = mockCourierService.deleteCourier;

jest.mock("@/bootstrapper/constants/IOCContainer", () => ({
    IOCContainer: {
        get: jest.fn(() => mockCourierService)
    }
}));

describe("useCourierListViewModel", () => {
    beforeEach(() => {
        mockNavigate.mockClear();
        mockCourierService.getCourierList$.mockClear();
        mockCourierService.deleteCourier.mockClear();
        mockCourierStore$.set({});
    });

    it("displayEditForm calls navigate", () => {
        const { result } = renderHook(() => useCourierListViewModel());
        result.current.displayEditForm("editForm");

        expect(mockNavigate).toHaveBeenCalledWith({
            pathname: '/(tabs)/(couriers)/edit',
            params: { courierId: "editForm" }
        });
    });

    it("deleteCourier calls courierServices.deleteCourier", () => {
        const { result } = renderHook(() => useCourierListViewModel());
        result.current.deleteCourier();

        expect(mockDeleteCourier).not.toHaveBeenCalled();

        act(() => {
            result.current.setCourierToDelete("mockedCourier");
        })

        result.current.deleteCourier();
        expect(mockDeleteCourier).toHaveBeenCalledTimes(1);
    });

    it("updates courierList when observable state changes", async () => {
        mockCourierStore$.set({
            "1": { id: "1", name: "Courier 1" },
            "2": { id: "2", name: "Courier 2" }
        });

        const { result } = renderHook(() => useCourierListViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(result.current.courierList).toHaveLength(2);
        expect(result.current.courierList).toEqual([
            { id: "1", name: "Courier 1" },
            { id: "2", name: "Courier 2" }
        ]);
    });

    it("handles null/undefined observable state", async () => {
        mockCourierStore$.set(null as any);

        const { result } = renderHook(() => useCourierListViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(result.current.courierList).toEqual([]);
    });
})