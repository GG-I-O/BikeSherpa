import Customer from "@/customers/models/Customer";
import useCustomerListViewModel from "@/customers/viewModels/useCustomerListViewModel";
import CustomerListView from "@/customers/views/CustomerListView";
import { hateoasRel } from "@/models/HateoasLink";
import { render, screen, userEvent, waitFor } from "@testing-library/react-native";
import { UserEventInstance } from "@testing-library/react-native/build/user-event/setup";
import { act } from "react";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/customers/viewModels/useCustomerListViewModel");
jest.mock("@/components/themed/ThemedConfirmationModal");
jest.mock("@expo/vector-icons", () => {
    const { Text } = require("react-native");
    return {
        MaterialIcons: Text,
    };
});


describe("CustomerListView", () => {
    let userAction: UserEventInstance;
    const mockCustomer1: Customer = {
        id: "123",
        name: "Existing Company",
        address: {
            name: "Existing Company",
            fullAddress: "10 rue de la Paix 75000 Paris",
            streetInfo: "10 rue de la Paix",
            complement: undefined,
            postcode: "75000",
            city: "Paris"
        },
        code: "EX1",
        phoneNumber: "0609080704",
        email: "existing.company@gmail.com",
        createdAt: "2024-01-01T00:00:00.000Z",
        updatedAt: "2024-01-01T00:00:00.000Z",
        links: [{
            href: "",
            rel: hateoasRel.update,
            method: ""
        },
        {
            href: "",
            rel: hateoasRel.delete,
            method: ""
        }]
    };

    const mockCustomer2: Customer = {
        id: "456",
        name: "Another Existing Company",
        address: {
            name: "Another Existing Company",
            fullAddress: "20 rue de la Paix 75000 Paris",
            streetInfo: "20 rue de la Paix",
            complement: undefined,
            postcode: "75000",
            city: "Paris"
        },
        code: "EX2",
        phoneNumber: "0609080704",
        email: "existing.company@gmail.com",
        createdAt: "2024-01-01T00:00:00.000Z",
        updatedAt: "2024-01-01T00:00:00.000Z",
        links: [{
            href: "",
            rel: hateoasRel.update,
            method: ""
        }]
    };

    const viewModel = jest.mocked(useCustomerListViewModel).mockReturnValue(
        {
            customerList: [mockCustomer1, mockCustomer2],
            displayEditForm: jest.fn(),
            deleteCustomer: jest.fn(),
            setCustomerToDelete: jest.fn()
        }
    )();

    beforeEach(() => {
        jest.useFakeTimers();
        userAction = userEvent.setup();
    })

    afterEach(() => {
        act(() => jest.runOnlyPendingTimers());
        jest.useRealTimers();
    });

    it("renders the view correctly", async () => {
        render(<CustomerListView />);

        await waitFor(() => {
            expect(screen.getByTestId("customerListView")).toBeOnTheScreen();
        });

        const customerListView = screen.getByTestId("customerListView");
        const telTitle = screen.queryByText("Num Tél");
        const customerData1 = screen.getByTestId("customerList0");
        const customerData2 = screen.getByTestId("customerList1");
        const customerOneName = screen.queryByText("Num Tél");
        const customerTwoCode = screen.queryByText("Num Tél");

        expect(customerListView).not.toBeNull();
        expect(customerData1).toBeOnTheScreen();
        expect(customerData2).toBeOnTheScreen();
        expect(telTitle).toBeOnTheScreen();
    })

    it("pressing the edit button displays the edit form", async () => {
        //arrange
        render(<CustomerListView />);

        //act
        await userAction.press(screen.getByTestId("editButton0"));
        act(() => jest.advanceTimersByTime(1000));

        //assert
        expect(viewModel.displayEditForm).toHaveBeenCalledTimes(1);
    })

    it("pressing the delete button calls setCustomerToDelete", async () => {
        //arrange
        render(<CustomerListView />);

        //act
        await userAction.press(screen.getByTestId("deleteButton1"));
        act(() => jest.advanceTimersByTime(1000));

        //assert
        expect(viewModel.setCustomerToDelete).toHaveBeenCalledTimes(1);
    })
})