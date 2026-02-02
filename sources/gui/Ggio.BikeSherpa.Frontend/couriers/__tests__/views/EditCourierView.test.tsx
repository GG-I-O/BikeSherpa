import CourierForm from "@/couriers/components/CourierForm";
import Courier from "@/couriers/models/Courier";
import { useEditCourierFormViewModel } from "@/couriers/viewModels/useEditCourierFormViewModel";
import EditCourierView from "@/couriers/views/EditCourierView";
import { render } from "@testing-library/react-native";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/couriers/viewModels/useEditCourierFormViewModel");

jest.mock("@/couriers/components/CourierForm", () => ({
    __esModule: true,
    default: jest.fn(() => null),
}));

describe("EditCourierView", () => {
    it("renders the view correctly", () => {
        //arrange
        const viewModel = jest.mocked(useEditCourierFormViewModel).mockReturnValue({
            control: {} as any,
            handleSubmit: jest.fn(),
            errors: {},
            setValue: jest.fn()
        });
        const courierForm = jest.mocked(CourierForm<Courier>);

        //act
        render(<EditCourierView />);

        //assert
        expect(courierForm).toHaveBeenCalled();
        expect(viewModel).toHaveBeenCalled();
    })
})