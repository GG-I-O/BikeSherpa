import CourierForm from "@/couriers/components/CourierForm";
import { render, renderHook, screen, userEvent } from '@testing-library/react-native';
import { useForm } from "react-hook-form";
import { UserEventInstance } from '@testing-library/react-native/build/user-event/setup';
import ThemedInputModule from '@/components/themed/ThemedInput';
import ThemedAddressInputModule from '@/components/themed/ThemedAddressInput';

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock("@/bootstrapper/constants/IOCContainer");

jest.mock('@/components/themed/ThemedInput', () => ({
    __esModule: true,
    default: jest.fn(() => null),
}));

jest.mock('@/components/themed/ThemedAddressInput', () => ({
    __esModule: true,
    default: jest.fn(() => null),
}));

const ThemedInput = jest.mocked(ThemedInputModule);
const ThemedAddressInput = jest.mocked(ThemedAddressInputModule);

describe("CourierForm", () => {
    let userAction: UserEventInstance;
    const { result } = renderHook(() => useForm());
    const mockedFunction = jest.fn();

    beforeEach(() => {
        ThemedInput.mockClear();
        ThemedAddressInput.mockClear();
        userAction = userEvent.setup();
    });

    it("CourierForm is rendering correctly", () => {
        render(<CourierForm
            control={result.current.control}
            errors={{}}
            handleSubmit={mockedFunction}
            buttonName="Modifier le livreur"
        />)

        const formButton = screen.getByTestId("formButton");
        const buttonName = screen.queryByTestId("buttonName");

        // Check that ThemedInput is called 6 times (firsname, lastName, code, email, phone, complement)
        expect(ThemedInput).toHaveBeenCalledTimes(6);

        // Get the props from each call
        const call1Props = ThemedInput.mock.calls[0][0];
        const call2Props = ThemedInput.mock.calls[1][0];
        const call3Props = ThemedInput.mock.calls[2][0];
        const call4Props = ThemedInput.mock.calls[3][0];
        const call5Props = ThemedInput.mock.calls[4][0];
        const call6Props = ThemedInput.mock.calls[5][0];

        // Check first call - FirstName input
        expect(call1Props.label).toBe("Prénom");
        expect(call1Props.placeholder).toBe("Jean-Claude");
        expect(call1Props.name).toBe("firstName");
        expect(call1Props.required).toBe(true);
        expect(call1Props.testID).toBe("courierFormFirstNameInput");

        // Check second call - LastName input
        expect(call2Props.label).toBe("Nom");
        expect(call2Props.placeholder).toBe("Dusse");
        expect(call2Props.name).toBe("lastName");
        expect(call2Props.required).toBe(true);
        expect(call2Props.testID).toBe("courierFormLastNameInput");

        // Check third call - Code input
        expect(call3Props.label).toBe("Code");
        expect(call3Props.placeholder).toBe("JCD");
        expect(call3Props.name).toBe("code");
        expect(call3Props.required).toBe(true);
        expect(call3Props.testID).toBe("courierFormCodeInput");

        // Check fourth call - Email input
        expect(call4Props.label).toBe("E-mail");
        expect(call4Props.placeholder).toBe("jean-claude-dusse@gmail.fr");
        expect(call4Props.name).toBe("email");
        expect(call4Props.required).toBe(true);
        expect(call4Props.testID).toBe("courierFormEmailInput");

        // Check fifth call - Phone input
        expect(call5Props.label).toBe("Téléphone");
        expect(call5Props.placeholder).toBe("06 10 11 12 13");
        expect(call5Props.name).toBe("phoneNumber");
        expect(call5Props.required).toBe(true);
        expect(call5Props.testID).toBe("courierFormPhoneInput");

        // Check sixth call - Complement input (not required)
        expect(call6Props.label).toBe("Complément d’adresse");
        expect(call6Props.placeholder).toBe("Bâtiment B");
        expect(call6Props.name).toBe("complement");
        expect(call6Props.required).toBeUndefined();
        expect(call6Props.testID).toBe("courierFormComplementInput");

        // Check ThemedAddressInput is called once
        expect(ThemedAddressInput).toHaveBeenCalledTimes(1);
        const addressProps = ThemedAddressInput.mock.calls[0][0];
        expect(addressProps.label).toBe("Adresse");
        expect(addressProps.placeholder).toBe("10 rue de la République 38100 Grenoble");
        expect(addressProps.name).toBe("address");
        expect(addressProps.required).toBe(true);

        // Check button
        expect(formButton).toBeOnTheScreen();
        expect(formButton).toContainElement(buttonName);
        expect(buttonName).toHaveTextContent("Modifier le livreur");
    })

    it("", async () => {
        render(<CourierForm
            control={result.current.control}
            errors={{}}
            handleSubmit={mockedFunction}
            buttonName="Modifier le livreur"
        />)
        const formButton = screen.queryByTestId("formButton");
        await userAction.press(formButton);
        expect(mockedFunction).toHaveBeenCalled();
    })
})