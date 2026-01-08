import { render, screen, fireEvent, waitFor, act, userEvent } from '@testing-library/react-native';
import AppSnackbar from "../../components/AppSnackbar";
import { UserEventInstance } from '@testing-library/react-native/build/user-event/setup';

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

describe("AppSnackbar", () => {
    let userAction: UserEventInstance;

    beforeEach(() => {
        jest.useFakeTimers();
        userAction = userEvent.setup();
    });

    afterEach(() => {
        act(() => jest.runOnlyPendingTimers());
        jest.useRealTimers();
    });

    it("AppSnackbar is rendering", () => {
        render(<AppSnackbar
            visible={true}
            text={"La snackbar s'affiche."}
            onDismiss={function (): void { }}
        />)
        const snackbar = screen.getByTestId("AppSnackbar");
        expect(snackbar).toBeOnTheScreen();
    });

    it("AppSnackbar close button displays fermer", () => {
        render(<AppSnackbar
            visible={true}
            text={"La snackbar s'affiche."}
            onDismiss={function (): void { }}
        />)
        const snackbar = screen.getByTestId("AppSnackbar");
        const closeButton = screen.queryByText("fermer");
        expect(closeButton).toBeOnTheScreen();
    });

    it("AppSnackbar is not rendering", () => {
        render(<AppSnackbar
            visible={false}
            text={"La snackbar ne s'affiche pas."}
            onDismiss={function (): void { }}
        />)

        expect(screen.queryByTestId("AppSnackbar")).toBeNull();
    });

    it("AppSnackbar onDismiss function is called after pressing close button", async () => {
        const mockedFunction = jest.fn();
        render(<AppSnackbar
            visible={true}
            text={"La snackbar s'affiche."}
            onDismiss={mockedFunction}
        />)

        const closeButton = screen.getByText("fermer");
        await userAction.press(closeButton);

        expect(mockedFunction).toHaveBeenCalled();
    });

    it("AppSnackbar onDismiss function is called after duration timeout", async () => {
        const mockedFunction = jest.fn();

        render(<AppSnackbar
            visible={true}
            text={"La snackbar s'affiche."}
            onDismiss={mockedFunction}
        />)

        expect(mockedFunction).not.toHaveBeenCalled();

        // Fast-forward time by 2000ms (the duration)
        act(() => {
            jest.advanceTimersByTime(2000);
        });
        await waitFor(() => {
            expect(mockedFunction).toHaveBeenCalledTimes(1);
        });
    });
});