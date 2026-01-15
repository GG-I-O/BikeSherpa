import { renderHook, act } from '@testing-library/react-native';
import { useEditCustomerFormViewModel } from '../../viewModels/useEditCustomerFormViewModel';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ICustomerService } from '@/spi/CustomerSPI';
import { Observable, observable } from '@legendapp/state';
import Customer from '@/customers/models/Customer';
import EditCustomerFormViewModel from '@/customers/viewModels/EditCustomerFormViewModel';
import { mock } from 'ts-jest-mocker';
import { z } from 'zod';

const mockViewModel = mock(EditCustomerFormViewModel);

jest.mock('@/bootstrapper/constants/IOCContainer');
jest.mock('../../viewModels/EditCustomerFormViewModel', () => {
    return jest.fn(() => mockViewModel);
});

const mockCustomerService = mock<ICustomerService>();

describe('useEditCustomerFormViewModel', () => {
    let mockCustomerStore$: any;
    let mockCustomer$: Observable<Customer>;
    const mockCustomer = mock(Customer);

    beforeEach(() => {
        mockCustomerStore$ = observable<Record<string, Customer>>({});
        mockCustomer$ = observable<Customer>(mockCustomer);
        mockCustomerService.getCustomerList$.mockReturnValue(mockCustomerStore$);
        mockCustomerService.getCustomer$.mockReturnValue(mockCustomer$);

        (IOCContainer.get as jest.Mock).mockReturnValue(mockCustomerService);

        // Setup mock view model methods
        mockViewModel.getEditCustomerSchema = jest.fn().mockReturnValue(z.object({}));
        mockViewModel.onSubmit = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('properly invokes onSubmit callback when form is submitted', async () => {
        const { result } = renderHook(() => useEditCustomerFormViewModel(mockCustomer.id));

        await act(async () => {
            await result.current.handleSubmit({ preventDefault: jest.fn() } as any);
        });

        expect(mockViewModel.onSubmit).toHaveBeenCalled();
    });

    it("updates customerList when observable state changes", async () => {
        mockCustomerStore$.set({
            "1": { id: "1", name: "Customer 1" },
            "2": { id: "2", name: "Customer 2" }
        });

        renderHook(() => useEditCustomerFormViewModel(mockCustomer.id));

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getEditCustomerSchema).toHaveBeenCalledWith(mockCustomer, [
            { id: "1", name: "Customer 1" },
            { id: "2", name: "Customer 2" }
        ]);
    });

    it("handles null/undefined observable state", async () => {
        mockCustomerStore$.set(null as any);

        renderHook(() => useEditCustomerFormViewModel(mockCustomer.id));

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getEditCustomerSchema).toHaveBeenCalledWith(mockCustomer, []);
    });
});