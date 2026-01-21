import { render, screen } from '@testing-library/react-native';
import AppSnackbarView from '../../views/AppSnackbarView';
import useAppSnackbarViewModel from '../../viewModel/useAppSnackbarViewModel';
import React from 'react';

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock('react-native-paper', () => {
    return {
        ...jest.requireActual('react-native-paper'),
        Portal: ({ children }: any) => children,
        Provider: ({ children }: any) => children,
    };
});

jest.mock('../../viewModel/useAppSnackbarViewModel');

describe('AppSnackbarView', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('renders AppSnackbarView with snackbar visible', () => {
        const mockOnDismiss = jest.fn();
        (useAppSnackbarViewModel as jest.Mock).mockReturnValue({
            visibility: true,
            message: 'Test message',
            onDismiss: mockOnDismiss
        });

        render(<AppSnackbarView />);

        const snackbar = screen.getByTestId('AppSnackbar');
        expect(snackbar).toBeOnTheScreen();
        expect(screen.getByText('Test message')).toBeOnTheScreen();
    });

    it('renders AppSnackbarView with snackbar hidden', () => {
        const mockOnDismiss = jest.fn();
        (useAppSnackbarViewModel as jest.Mock).mockReturnValue({
            visibility: false,
            message: '',
            onDismiss: mockOnDismiss
        });

        render(<AppSnackbarView />);

        expect(screen.queryByTestId('AppSnackbar')).toBeNull();
    });

    it('renders AppSnackbarView and calls view model hook', () => {
        const mockOnDismiss = jest.fn();
        (useAppSnackbarViewModel as jest.Mock).mockReturnValue({
            visibility: false,
            message: '',
            onDismiss: mockOnDismiss
        });

        render(<AppSnackbarView />);

        expect(useAppSnackbarViewModel).toHaveBeenCalled();
    });

    it('renders AppSnackbarView with different messages', () => {
        const mockOnDismiss = jest.fn();
        (useAppSnackbarViewModel as jest.Mock).mockReturnValue({
            visibility: true,
            message: 'Une erreur est survenue',
            onDismiss: mockOnDismiss
        });

        render(<AppSnackbarView />);

        expect(screen.getByText('Une erreur est survenue')).toBeOnTheScreen();
    });
});
