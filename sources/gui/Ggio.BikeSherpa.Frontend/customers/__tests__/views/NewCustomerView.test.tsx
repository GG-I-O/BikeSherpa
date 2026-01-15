import CustomerForm from "@/customers/components/CustomerForm";
import InputCustomer from "@/customers/models/InputCustomer";
import { useNewCustomerFormViewModel } from "@/customers/viewModels/useNewCustomerFormViewModel";
import NewCustomerView from "@/customers/views/NewCustomerView";
import { render } from "@testing-library/react-native";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/customers/viewModels/useNewCustomerFormViewModel");

jest.mock("@/customers/components/CustomerForm", () => ({
    __esModule: true,
    default: jest.fn(() => null),
}));

describe("NewCustomerForm", () => {
    it("renders the view correctly", () => {
        //arrange
        const viewModel = jest.mocked(useNewCustomerFormViewModel).mockReturnValue({
            control: {} as any,
            handleSubmit: jest.fn(),
            errors: {}
        });
        const customerForm = jest.mocked(CustomerForm<InputCustomer>);

        //act
        render(<NewCustomerView />);

        //assert
        expect(customerForm).toHaveBeenCalled();
        expect(viewModel).toHaveBeenCalled();
    })
})