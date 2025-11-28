import * as ReactHookForm from 'react-hook-form';
import { act, render, screen, userEvent, waitFor } from '@testing-library/react-native';
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer"
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import { Address } from "@/models/Address";

const mockAddress1: Address = {
    name: "Société 1",
    streetInfo: "10 rue de la Société 1",
    postcode: "38000",
    city: "Grenoble"
}
const mockAddress2: Address = {
    name: "Société 2",
    streetInfo: "10 rue de la Société 2",
    postcode: "38300",
    city: "Grenoble"
}
const mockAddress3: Address = {
    name: "Société 3",
    streetInfo: "10 rue de la Société 3",
    postcode: "38100",
    city: "Échirolles"
}

const mockOnChange = jest.fn((newValue) => {
    mockField.value = newValue;
});
const mockFieldValue = { name: '' };
const mockFieldName = 'name';

const mockField = {
    name: mockFieldName,
    value: mockFieldValue,
    onChange: mockOnChange,
    onBlur: jest.fn(),
    ref: jest.fn()
};

const mockAddressService = {
    fetchAddress: jest.fn()
};

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({ top: 0, bottom: 0, left: 0, right: 0 })
}));

jest.mock('react-hook-form', () => ({
    ...jest.requireActual('react-hook-form'),
    useController: jest.fn()
}));

jest.mock("@/bootstrapper/constants/IOCContainer");


describe("ThemedAddressInput", () => {

    beforeEach(() => {
        jest.clearAllMocks();
        jest.useFakeTimers();

        (ReactHookForm.useController as jest.Mock).mockReturnValue({
            field: mockField,
            fieldState: {} as any,
            formState: {} as any
        });

        (IOCContainer.get as jest.Mock).mockReturnValue(mockAddressService);
    });

    afterEach(() => {
        act(() => jest.runOnlyPendingTimers());
        jest.useRealTimers();
        jest.restoreAllMocks();
    });

    it("renders a required component correctly", () => {

        // Arrange

        // Act
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="Adresse"
                placeholder="10 avenue de la Gare"
                required
            />
        );

        // Assert
        expect(screen.getByTestId('themedAddressInputLabel').children).toContain('Adresse');
        expect(screen.getByTestId('themedAddressInputTextInput').props.placeholder).toBe("10 avenue de la Gare");
    });

    it("does not render a list of addresse on invalid input", async () => {
        // Arrange
        const user = userEvent.setup();

        // Act
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="Adresse"
                placeholder="10 avenue de la Gare"
                required
            />
        );
        await user.type(screen.getByTestId("themedAddressInputTextInput"), 'R u');
        act(() => jest.advanceTimersByTime(1000));

        // Assert
        expect(mockAddressService.fetchAddress).not.toHaveBeenCalled();
        const addressList = screen.queryByTestId("themedAddressInputAddressList");
        expect(addressList).toBeNull();
    });

    it("renders a list of addresses on text input", async () => {
        // Arrange
        const user = userEvent.setup();
        const mockAddresses = [mockAddress1, mockAddress2, mockAddress3];
        (mockAddressService.fetchAddress as
            jest.MockedFunction<(text: string) => Promise<Address[]>>)
            .mockResolvedValue(mockAddresses as Address[]);

        // Act
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="Adresse"
                placeholder="10 avenue de la Gare"
                required
            />
        );
        await user.type(screen.getByTestId("themedAddressInputTextInput"), 'Rue de la paix');
        act(() => jest.advanceTimersByTime(1000));

        // Assert
        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalledWith('Rue de la paix');
        });

        const addressList = screen.getByTestId("themedAddressInputAddressList");
        expect(addressList).toBeOnTheScreen();
    });

    // it("fills the input field when an address suggestion is clicked", () => {

    // });

    // it("hides suggestions when an address suggestion is clicked", () => {

    // });
})