import ThemedConfirmationModal from "@/components/themed/ThemedConfirmationModal";
import Customer from "@/customers/models/Customer";
import useCustomerListViewModel from "@/customers/viewModels/useCustomerListViewModel";
import CustomerListView from "@/customers/views/CustomerListView";
import { createRandomCustomer, linkType } from "@/fixtures/customer-fixtures";
import { render, screen, userEvent, waitFor } from "@testing-library/react-native";
import { UserEventInstance } from "@testing-library/react-native/build/user-event/setup";
import { act } from "react";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/customers/viewModels/useCustomerListViewModel");

jest.mock("@/components/themed/ThemedConfirmationModal", () => {
    return {
        __esModule: true,
        default: jest.fn()
    }
});

jest.mock("@expo/vector-icons", () => {
    const { Text } = jest.requireActual("react-native");
    return {
        MaterialIcons: Text,
    };
});

describe("CustomerListView", () => {
    let userAction: UserEventInstance;
    const mockCustomer1: Customer = createRandomCustomer(true, linkType.updateAndDelete);
    const mockCustomer2: Customer = createRandomCustomer(true, linkType.update);

    let mockDisplayEditForm: jest.Mock;
    let mockDeleteCustomer: jest.Mock;
    let mockSetCustomerToDelete: jest.Mock;

    beforeEach(() => {
        jest.clearAllMocks();
        jest.useFakeTimers();
        userAction = userEvent.setup();

        mockDisplayEditForm = jest.fn();
        mockDeleteCustomer = jest.fn();
        mockSetCustomerToDelete = jest.fn();

        jest.mocked(useCustomerListViewModel).mockReturnValue({
            customerList: [mockCustomer1, mockCustomer2],
            displayEditForm: mockDisplayEditForm,
            deleteCustomer: mockDeleteCustomer,
            setCustomerToDelete: mockSetCustomerToDelete
        });
    })

    afterEach(() => {
        act(() => jest.runOnlyPendingTimers());
        jest.useRealTimers();
    });

    it("renders the view correctly", async () => {
        //arrange
        render(<CustomerListView />);

        //act
        await waitFor(() => {
            expect(screen.getByTestId("customerListView")).toBeOnTheScreen();
        });
        const customerListView = screen.getByTestId("customerListView");
        const telTitle = screen.queryByText("Num Tél");
        const customerData1 = screen.getByTestId("customerList0");
        const customerData2 = screen.getByTestId("customerList1");
        const customerOneName = screen.queryByText(mockCustomer1.name);
        const customerTwoCode = screen.queryByText(mockCustomer2.code);

        //assert
        expect(customerListView).not.toBeNull();
        expect(customerData1).toBeOnTheScreen();
        expect(customerData2).toBeOnTheScreen();
        expect(telTitle).toBeOnTheScreen();
        expect(customerOneName).toBeOnTheScreen();
        expect(customerTwoCode).toBeOnTheScreen();
    })

    it("pressing the edit button displays the edit form", async () => {
        //arrange
        render(<CustomerListView />);

        //act
        await userAction.press(screen.getByTestId("editButton0"));
        act(() => jest.advanceTimersByTime(1000));

        //assert
        expect(mockDisplayEditForm).toHaveBeenCalledTimes(1);
    })

    it("pressing the delete button displays the confirmation modal and calls setCustomerToDelete", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);
        render(<CustomerListView />);

        //act
        const deleteButton1 = screen.getByTestId("deleteButton1");
        await userAction.press(deleteButton1);
        act(() => jest.advanceTimersByTime(1000));

        //assert
        expect(confirmationModal).toHaveBeenCalledTimes(2);
        const call1Props = confirmationModal.mock.calls[0][0];
        const call2Props = confirmationModal.mock.calls[1][0];
        expect(mockSetCustomerToDelete).toHaveBeenCalledTimes(1);
        expect(call1Props.visible).toBeFalsy();
        expect(call2Props.visible).toBeTruthy();
    })

    it("modal is on the screen on initial render", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);

        // act
        render(<CustomerListView />);

        //assert
        expect(confirmationModal).toHaveBeenCalledTimes(1);
        const call1Props = confirmationModal.mock.calls[0][0];
        expect(call1Props.visible).toBeFalsy();
    })

    it("renders customer with undefined address fields correctly", async () => {
        //arrange
        const mockCustomerWithNoAddress: Customer = {
            ...mockCustomer1,
            name: "Company Without Address",
            address: {
                name: "Company Without Address",
                fullAddress: "",
                streetInfo: undefined as any,
                complement: null,
                postcode: undefined as any,
                city: undefined as any
            }
        };

        jest.mocked(useCustomerListViewModel).mockReturnValue({
            customerList: [mockCustomerWithNoAddress],
            displayEditForm: jest.fn(),
            deleteCustomer: jest.fn(),
            setCustomerToDelete: jest.fn()
        });

        //act
        render(<CustomerListView />);

        //assert
        await waitFor(() => {
            expect(screen.getByText("Company Without Address")).toBeOnTheScreen();
        });
    })

    it("calls deleteCustomer and closes modal when confirmButton is called", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);
        render(<CustomerListView />);

        //act
        const deleteButton = screen.getByTestId("deleteButton0");
        await userAction.press(deleteButton);
        act(() => jest.advanceTimersByTime(1000));
        const modalProps = confirmationModal.mock.calls[1][0];
        act(() => {
            modalProps.confirmButton();
        });

        //assert
        expect(mockDeleteCustomer).toHaveBeenCalledTimes(1);
        const updatedModalProps = confirmationModal.mock.calls[2][0];
        expect(updatedModalProps.visible).toBeFalsy();
    })

    it("closes modal when cancelButton is called", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);
        render(<CustomerListView />);

        //act
        const deleteButton = screen.getByTestId("deleteButton0");
        await userAction.press(deleteButton);
        act(() => jest.advanceTimersByTime(1000));
        const modalProps = confirmationModal.mock.calls[1][0];
        act(() => {
            modalProps.cancelButton();
        });

        //assert
        expect(mockDeleteCustomer).not.toHaveBeenCalled();
        const updatedModalProps = confirmationModal.mock.calls[2][0];
        expect(updatedModalProps.visible).toBeFalsy();
    })
})