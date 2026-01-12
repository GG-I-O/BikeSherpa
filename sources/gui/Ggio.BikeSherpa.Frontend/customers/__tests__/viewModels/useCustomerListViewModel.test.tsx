import useCustomerListViewModel from "@/customers/viewModels/useCustomerListViewModel";
import { observable } from "@legendapp/state";
import { act, renderHook } from '@testing-library/react-native';
import { navigate } from "expo-router/build/global-state/routing";

jest.mock("expo-router/build/global-state/routing", () => ({
    navigate: jest.fn()
}));

const mockNavigate = navigate as jest.Mock;
const mockCustomerStore$ = observable({});
const mockCustomerService = {
    getCustomerList$: jest.fn(() => mockCustomerStore$),
    deleteCustomer: jest.fn()
};
const mockDeleteCustomer = mockCustomerService.deleteCustomer;

jest.mock("@/bootstrapper/constants/IOCContainer", () => ({
    IOCContainer: {
        get: jest.fn(() => mockCustomerService)
    }
}));

describe("useCustomerListViewModel", () => {
    beforeEach(() => {
        mockNavigate.mockClear();
        mockCustomerService.getCustomerList$.mockClear();
        mockCustomerService.deleteCustomer.mockClear();
        mockCustomerStore$.set({});
    });

    it("displayEditForm calls navigate", () => {
        const { result } = renderHook(() => useCustomerListViewModel());
        result.current.displayEditForm("editForm");

        expect(mockNavigate).toHaveBeenCalledWith({
            pathname: '/(tabs)/(customers)/edit',
            params: { customerId: "editForm" }
        });
    });

    it("deleteCustomer calls customerServices.deleteCustomer", () => {
        const { result } = renderHook(() => useCustomerListViewModel());
        result.current.deleteCustomer();

        expect(mockDeleteCustomer).not.toHaveBeenCalled();

        act(() => {
            result.current.setCustomerToDelete("mockedCustomer");
        })

        result.current.deleteCustomer();
        expect(mockDeleteCustomer).toHaveBeenCalledTimes(1);
    });

    it("updates customerList when observable state changes", async () => {
        mockCustomerStore$.set({
            "1": { id: "1", name: "Customer 1" },
            "2": { id: "2", name: "Customer 2" }
        });

        const { result } = renderHook(() => useCustomerListViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(result.current.customerList).toHaveLength(2);
        expect(result.current.customerList).toEqual([
            { id: "1", name: "Customer 1" },
            { id: "2", name: "Customer 2" }
        ]);
    });

    it("handles null/undefined observable state", async () => {
        mockCustomerStore$.set(null as any);

        const { result } = renderHook(() => useCustomerListViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(result.current.customerList).toEqual([]);
    });
}) 