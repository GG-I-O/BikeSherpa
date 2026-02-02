import CourierForm from "@/couriers/components/CourierForm";
import InputCourier from "@/couriers/models/InputCourier";
import { useNewCourierFormViewModel } from "@/couriers/viewModels/useNewCourierFormViewModel";
import NewCourierView from "@/couriers/views/NewCourierView";
import { render } from "@testing-library/react-native";

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/couriers/viewModels/useNewCourierFormViewModel");

jest.mock("@/couriers/components/CourierForm", () => ({
    __esModule: true,
    default: jest.fn(() => null),
}));

describe("NewCourierView", () => {
    it("renders the view correctly", () => {
        //arrange
        const viewModel = jest.mocked(useNewCourierFormViewModel).mockReturnValue({
            control: {} as any,
            handleSubmit: jest.fn(),
            errors: {}
        });
        const courierForm = jest.mocked(CourierForm<InputCourier>);

        //act
        render(<NewCourierView />);

        //assert
        expect(courierForm).toHaveBeenCalled();
        expect(viewModel).toHaveBeenCalled();
    })
})