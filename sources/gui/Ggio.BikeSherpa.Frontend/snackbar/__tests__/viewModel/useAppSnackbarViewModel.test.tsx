import { renderHook, act, waitFor } from '@testing-library/react-native';
import useAppSnackbarViewModel from '../../viewModel/useAppSnackbarViewModel';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';

jest.mock('@/bootstrapper/constants/IOCContainer');

describe('useAppSnackbarViewModel', () => {
    let mockSubscribe: jest.Mock;
    let mockUnsubscribe: jest.Mock;
    let subscriptionCallback: ((message: string) => void) | null = null;

    beforeEach(() => {
        jest.useFakeTimers();
        subscriptionCallback = null;

        mockSubscribe = jest.fn((callback) => {
            subscriptionCallback = callback;
            return 'subscription-id';
        });

        mockUnsubscribe = jest.fn();

        (IOCContainer.get as jest.Mock).mockReturnValue({
            subscribe: mockSubscribe,
            unSubscribe: mockUnsubscribe
        });
    });

    afterEach(() => {
        jest.useRealTimers();
        jest.clearAllMocks();
    });

    it('initializes with visibility false and empty message', () => {
        const { result } = renderHook(() => useAppSnackbarViewModel());

        expect(result.current.visibility).toBe(false);
        expect(result.current.message).toBe('');
    });

    it('subscribes to appSnackbarService on mount', () => {
        renderHook(() => useAppSnackbarViewModel());

        expect(IOCContainer.get).toHaveBeenCalledWith(ServicesIdentifiers.AppSnackbarService);
        expect(mockSubscribe).toHaveBeenCalled();
    });

    it('displays message when service sends one', async () => {
        const { result } = renderHook(() => useAppSnackbarViewModel());

        act(() => {
            subscriptionCallback?.('Test message');
        });

        await waitFor(() => {
            expect(result.current.visibility).toBe(true);
            expect(result.current.message).toBe('Test message');
        });
    });

    it('queues multiple messages and displays them one at a time', async () => {
        const { result } = renderHook(() => useAppSnackbarViewModel());

        act(() => {
            subscriptionCallback?.('First message');
            subscriptionCallback?.('Second message');
        });

        await waitFor(() => {
            expect(result.current.visibility).toBe(true);
            expect(result.current.message).toBe('First message');
        });
    });

    it('onDismiss shows next message after delay', async () => {
        const { result } = renderHook(() => useAppSnackbarViewModel());

        // Send two messages
        act(() => {
            subscriptionCallback?.('First message');
            subscriptionCallback?.('Second message');
        });

        await waitFor(() => {
            expect(result.current.message).toBe('First message');
            expect(result.current.visibility).toBe(true);
        });

        // Dismiss the first message and advance timer
        act(() => {
            result.current.onDismiss();
            jest.advanceTimersByTime(300);
        });

        await waitFor(() => {
            expect(result.current.message).toBe('Second message');
            expect(result.current.visibility).toBe(true);
        });
    });

    it('onDismiss keeps visibility false when no more messages', async () => {
        const { result } = renderHook(() => useAppSnackbarViewModel());

        // Send one message
        act(() => {
            subscriptionCallback?.('Only message');
        });

        await waitFor(() => {
            expect(result.current.visibility).toBe(true);
        });

        // Dismiss the message
        act(() => {
            result.current.onDismiss();
            jest.advanceTimersByTime(300);
        });

        await waitFor(() => {
            expect(result.current.visibility).toBe(false);
        });
    });
});
