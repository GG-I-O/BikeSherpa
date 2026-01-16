import CustomerForm from "@/customers/components/CustomerForm";
import Customer from "@/customers/models/Customer";
import { useEditCustomerFormViewModel } from "@/customers/viewModels/useEditCustomerFormViewModel";
import EditCustomerView from "@/customers/views/EditCustomerView";
import { render } from "@testing-library/react-native";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/customers/viewModels/useEditCustomerFormViewModel");

jest.mock("@/customers/components/CustomerForm", () => ({
    __esModule: true,
    default: jest.fn(() => null),
}));

describe("EditCustomerView", () => {
    it("renders the view correctly", () => {
        //arrange
        const viewModel = jest.mocked(useEditCustomerFormViewModel).mockReturnValue({
            control: {} as any,
            handleSubmit: jest.fn(),
            errors: {},
            setValue: jest.fn()
        });
        const customerForm = jest.mocked(CustomerForm<Customer>);

        //act
        render(<EditCustomerView />);

        //assert
        expect(customerForm).toHaveBeenCalled();
        expect(viewModel).toHaveBeenCalled();
    })
})