import { renderHook, act } from '@testing-library/react-native';
import { useNewCustomerFormViewModel } from '../../viewModels/useNewCustomerFormViewModel';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ICustomerService } from '@/spi/CustomerSPI';
import { observable } from '@legendapp/state';
import Customer from '@/customers/models/Customer';
import NewCustomerFormViewModel from '../../viewModels/NewCustomerFormViewModel';
import { mock } from 'ts-jest-mocker';
import { z } from 'zod';

const mockViewModel = mock(NewCustomerFormViewModel);

jest.mock('@/bootstrapper/constants/IOCContainer');
jest.mock('../../viewModels/NewCustomerFormViewModel', () => {
    return jest.fn(() => mockViewModel);
});

const mockCustomerService = mock<ICustomerService>();

describe('useNewCustomerFormViewModel', () => {
    let mockCustomerStore$: any;

    beforeEach(() => {
        mockCustomerStore$ = observable<Record<string, Customer>>({});
        mockCustomerService.getCustomerList$.mockReturnValue(mockCustomerStore$);

        (IOCContainer.get as jest.Mock).mockReturnValue(mockCustomerService);

        // Setup mock view model methods
        mockViewModel.getNewCustomerSchema = jest.fn().mockReturnValue(z.object({}));
        mockViewModel.setResetCallback = jest.fn();
        mockViewModel.onSubmit = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('properly invokes onSubmit callback when form is submitted', async () => {
        const { result } = renderHook(() => useNewCustomerFormViewModel());

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

        const { result } = renderHook(() => useNewCustomerFormViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getNewCustomerSchema).toHaveBeenCalledWith([
            { id: "1", name: "Customer 1" },
            { id: "2", name: "Customer 2" }
        ]);
    });

    it("handles null/undefined observable state", async () => {
        mockCustomerStore$.set(null as any);

        const { result } = renderHook(() => useNewCustomerFormViewModel());

        await act(async () => {
            await new Promise(resolve => setTimeout(resolve, 10));
        });

        expect(mockViewModel.getNewCustomerSchema).toHaveBeenCalledWith([]);
    });
});