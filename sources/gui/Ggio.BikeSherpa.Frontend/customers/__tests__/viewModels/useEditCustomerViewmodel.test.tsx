import useCustomerListViewModel from "@/customers/viewModels/useCustomerListViewModel";
import { useEditCustomerFormViewModel } from "@/customers/viewModels/useEditCustomerFormViewModel";
import { observable } from "@legendapp/state";
import { act, renderHook } from '@testing-library/react-native';

jest.mock("@/bootstrapper/constants/IOCContainer", () => ({
    IOCContainer: {
        get: jest.fn(() => mockCustomerService)
    }
}));

const mockCustomerStore$ = observable({});
const mockCustomerService = {
    getCustomerList$: jest.fn(() => mockCustomerStore$),
    updateCustomer: jest.fn()
};

describe("useEditCustomerViewModel", () => {
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