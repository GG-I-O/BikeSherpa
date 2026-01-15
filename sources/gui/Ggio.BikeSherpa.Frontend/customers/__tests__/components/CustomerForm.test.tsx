import CustomerForm from "@/customers/components/CustomerForm";
import { render, renderHook, screen, userEvent } from '@testing-library/react-native';
import { useForm } from "react-hook-form";
import { UserEventInstance } from '@testing-library/react-native/build/user-event/setup';

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

describe("CustomerForm", () => {
    let userAction: UserEventInstance;
    const { result } = renderHook(() => useForm());
    const mockedFunction = jest.fn();
    const ThemedInput = require('@/components/themed/ThemedInput').default;
    const ThemedAddressInput = require('@/components/themed/ThemedAddressInput').default;

    beforeEach(() => {
        ThemedInput.mockClear();
        ThemedAddressInput.mockClear();
        userAction = userEvent.setup();
    });

    it("CustomerForm is rendering correctly", () => {
        render(<CustomerForm
            control={result.current.control}
            errors={{}}
            handleSubmit={mockedFunction}
            buttonName="Modifier le client"
        />)

        const formButton = screen.getByTestId("formButton");
        const buttonName = screen.queryByTestId("buttonName");

        // Check that ThemedInput is called 5 times (name, code, email, phone, complement)
        expect(ThemedInput).toHaveBeenCalledTimes(5);

        // Get the props from each call
        const call1Props = ThemedInput.mock.calls[0][0];
        const call2Props = ThemedInput.mock.calls[1][0];
        const call3Props = ThemedInput.mock.calls[2][0];
        const call4Props = ThemedInput.mock.calls[3][0];
        const call5Props = ThemedInput.mock.calls[4][0];

        // Check first call - Name input
        expect(call1Props.label).toBe("Nom");
        expect(call1Props.placeholder).toBe("Ma Petite Société");
        expect(call1Props.name).toBe("name");
        expect(call1Props.required).toBe(true);
        expect(call1Props.testID).toBe("customerFormNameInput");

        // Check second call - Code input
        expect(call2Props.label).toBe("Code");
        expect(call2Props.placeholder).toBe("MPS");
        expect(call2Props.name).toBe("code");
        expect(call2Props.required).toBe(true);
        expect(call2Props.testID).toBe("customerFormCodeInput");

        // Check third call - Email input
        expect(call3Props.label).toBe("E-mail");
        expect(call3Props.placeholder).toBe("votre-nom@societe.fr");
        expect(call3Props.name).toBe("email");
        expect(call3Props.required).toBe(true);
        expect(call3Props.testID).toBe("customerFormEmailInput");

        // Check fourth call - Phone input
        expect(call4Props.label).toBe("Téléphone");
        expect(call4Props.placeholder).toBe("06 10 11 12 13");
        expect(call4Props.name).toBe("phoneNumber");
        expect(call4Props.required).toBe(true);
        expect(call4Props.testID).toBe("customerFormPhoneInput");

        // Check fifth call - Complement input (not required)
        expect(call5Props.label).toBe("Complément d'adresse");
        expect(call5Props.placeholder).toBe("Bâtiment B");
        expect(call5Props.name).toBe("complement");
        expect(call5Props.required).toBeUndefined();
        expect(call5Props.testID).toBe("customerFormComplementInput");

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
        expect(buttonName).toHaveTextContent("Modifier le client");
    })

    it("", async () => {
        render(<CustomerForm
            control={result.current.control}
            errors={{}}
            handleSubmit={mockedFunction}
            buttonName="Modifier le client"
        />)
        const formButton = screen.queryByTestId("formButton");
        await userAction.press(formButton);
        expect(mockedFunction).toHaveBeenCalled();
    })
})