import ThemedConfirmationModal from "@/components/themed/ThemedConfirmationModal";
import Courier from "@/couriers/models/Courier";
import useCourierListViewModel from "@/couriers/viewModels/useCourierListViewModel";
import CourierListView from "@/couriers/views/CourierListView";
import { createRandomCourier, linkType } from "@/fixtures/courier-fixtures";
import { render, screen, userEvent, waitFor } from "@testing-library/react-native";
import { UserEventInstance } from "@testing-library/react-native/build/user-event/setup";
import { act } from "react";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/couriers/viewModels/useCourierListViewModel");

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

describe("CourierListView", () => {
    let userAction: UserEventInstance;
    const mockCourier1: Courier = createRandomCourier(true, linkType.updateAndDelete);
    const mockCourier2: Courier = createRandomCourier(true, linkType.update);

    let mockDisplayEditForm: jest.Mock;
    let mockDeleteCourier: jest.Mock;
    let mockSetCourierToDelete: jest.Mock;

    beforeEach(() => {
        jest.clearAllMocks();
        jest.useFakeTimers();
        userAction = userEvent.setup();

        mockDisplayEditForm = jest.fn();
        mockDeleteCourier = jest.fn();
        mockSetCourierToDelete = jest.fn();

        jest.mocked(useCourierListViewModel).mockReturnValue({
            courierList: [mockCourier1, mockCourier2],
            displayEditForm: mockDisplayEditForm,
            deleteCourier: mockDeleteCourier,
            setCourierToDelete: mockSetCourierToDelete
        });
    })

    afterEach(() => {
        act(() => jest.runOnlyPendingTimers());
        jest.useRealTimers();
    });

    it("renders the view correctly", async () => {
        //arrange
        render(<CourierListView />);

        //act
        await waitFor(() => {
            expect(screen.getByTestId("courierListView")).toBeOnTheScreen();
        });
        const courierListView = screen.getByTestId("courierListView");
        const telTitle = screen.queryByText("Téléphone");
        const courierData1 = screen.getByTestId("courierList0");
        const courierData2 = screen.getByTestId("courierList1");
        const courierOneFirstName = screen.queryByText(mockCourier1.firstName);
        const courierTwoLastName = screen.queryByText(mockCourier2.lastName);

        //assert
        expect(courierListView).not.toBeNull();
        expect(courierData1).toBeOnTheScreen();
        expect(courierData2).toBeOnTheScreen();
        expect(telTitle).toBeOnTheScreen();
        expect(courierOneFirstName).toBeOnTheScreen();
        expect(courierTwoLastName).toBeOnTheScreen();
    })

    it("pressing the edit button displays the edit form", async () => {
        //arrange
        render(<CourierListView />);

        //act
        await userAction.press(screen.getByTestId("editButton0"));
        act(() => jest.advanceTimersByTime(1000));

        //assert
        expect(mockDisplayEditForm).toHaveBeenCalledTimes(1);
    })

    it("pressing the delete button displays the confirmation modal and calls setCourierToDelete", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);
        render(<CourierListView />);

        //act
        const deleteButton1 = screen.getByTestId("deleteButton1");
        await userAction.press(deleteButton1);
        act(() => jest.advanceTimersByTime(1000));

        //assert
        expect(confirmationModal).toHaveBeenCalledTimes(2);
        const call1Props = confirmationModal.mock.calls[0][0];
        const call2Props = confirmationModal.mock.calls[1][0];
        expect(mockSetCourierToDelete).toHaveBeenCalledTimes(1);
        expect(call1Props.visible).toBeFalsy();
        expect(call2Props.visible).toBeTruthy();
    })

    it("modal is on the screen on initial render", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);

        // act
        render(<CourierListView />);

        //assert
        expect(confirmationModal).toHaveBeenCalledTimes(1);
        const call1Props = confirmationModal.mock.calls[0][0];
        expect(call1Props.visible).toBeFalsy();
    })

    it("renders courier with undefined address fields correctly", async () => {
        //arrange
        const mockCourierWithNoAddress: Courier = {
            ...mockCourier1,
            firstName: "Courier",
            lastName: "Without Address",
            address: {
                name: `${mockCourier1.firstName} ${mockCourier1.lastName}`,
                fullAddress: "",
                streetInfo: undefined as any,
                complement: null,
                postcode: undefined as any,
                city: undefined as any
            }
        };

        jest.mocked(useCourierListViewModel).mockReturnValue({
            courierList: [mockCourierWithNoAddress],
            displayEditForm: jest.fn(),
            deleteCourier: jest.fn(),
            setCourierToDelete: jest.fn()
        });

        //act
        render(<CourierListView />);

        //assert
        await waitFor(() => {
            expect(screen.getByText("Courier")).toBeOnTheScreen();
            expect(screen.getByText("Without Address")).toBeOnTheScreen();
        });
    })

    it("calls deleteCourier and closes modal when confirmButton is called", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);
        render(<CourierListView />);

        //act
        const deleteButton = screen.getByTestId("deleteButton0");
        await userAction.press(deleteButton);
        act(() => jest.advanceTimersByTime(1000));
        const modalProps = confirmationModal.mock.calls[1][0];
        act(() => {
            modalProps.confirmButton();
        });

        //assert
        expect(mockDeleteCourier).toHaveBeenCalledTimes(1);
        const updatedModalProps = confirmationModal.mock.calls[2][0];
        expect(updatedModalProps.visible).toBeFalsy();
    })

    it("closes modal when cancelButton is called", async () => {
        //arrange
        const confirmationModal = jest.mocked(ThemedConfirmationModal);
        render(<CourierListView />);

        //act
        const deleteButton = screen.getByTestId("deleteButton0");
        await userAction.press(deleteButton);
        act(() => jest.advanceTimersByTime(1000));
        const modalProps = confirmationModal.mock.calls[1][0];
        act(() => {
            modalProps.cancelButton();
        });

        //assert
        expect(mockDeleteCourier).not.toHaveBeenCalled();
        const updatedModalProps = confirmationModal.mock.calls[2][0];
        expect(updatedModalProps.visible).toBeFalsy();
    })
})