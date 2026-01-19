import * as ReactHookForm from 'react-hook-form';
import { act, render, screen, userEvent, waitFor } from '@testing-library/react-native';
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import { Address } from "@/models/Address";
import { UserEventInstance } from '@testing-library/react-native/build/user-event/setup';
import { useTheme, TextInput as PaperTextInput } from 'react-native-paper';
import { faker } from '@faker-js/faker';
import { createRandomAddress } from '@/fixtures/address-fixtures';

const mockAddresses: Address[] = faker.helpers.multiple(() => createRandomAddress(faker.company.name()), { count: 3 });

const mockField = {
    name: 'address',
    value: { name: '', streetInfo: '', postCode: '', city: '' },
    onChange: jest.fn((newValue) => { mockField.value = newValue; }),
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

jest.mock('react-native-paper', () => {
    return {
        ...jest.requireActual('react-native-paper'),
        useTheme: jest.fn()
    };
});

jest.mock("@/bootstrapper/constants/IOCContainer");


describe("ThemedAddressInput", () => {
    let userAction: UserEventInstance;
    const mockUseTheme = useTheme as jest.Mock;

    beforeEach(() => {
        jest.clearAllMocks();
        jest.useFakeTimers();

        userAction = userEvent.setup();

        mockUseTheme.mockReturnValue({
            fonts: {},
            colors: {}
        });

        mockField.value = { name: '', streetInfo: '', postCode: '', city: '' };
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
                label="LabelAddress"
                placeholder="10 avenue de la Gare"
                required
            />
        );

        // Assert
        const label = screen.getByTestId('themedAddressInputLabel');
        expect(label.children).toContain('LabelAddress');
        const requiredText = screen.getByText('*');
        expect(requiredText).toBeOnTheScreen();

        expect(screen.getByTestId('themedAddressInputTextInput').props.placeholder).toBe("10 avenue de la Gare");
    });

    it("renders a non required component correctly", () => {
        // Arrange

        // Act
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="LabelAddress"
                placeholder="10 avenue de la Gare"
            />
        );

        // Assert
        const label = screen.getByTestId('themedAddressInputLabel');
        expect(label.children).toContain('LabelAddress');
        const requiredText = screen.queryByText('*');
        expect(requiredText).toBeNull();

        expect(screen.getByTestId('themedAddressInputTextInput').props.placeholder).toBe("10 avenue de la Gare");
    });

    it("renders an error in component correctly", () => {
        // Arrange
        mockUseTheme.mockReturnValue({
            colors: { error: 'rgba(216, 133, 133, 1)' },
            fonts: {}
        });

        // Act
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="LabelAddress"
                placeholder="10 avenue de la Gare"
                error={{ type: 'required', message: 'Erreur Adresse' }}
            />
        );

        // Assert
        const errorText = screen.queryByText('Erreur Adresse');
        expect(errorText).toBeOnTheScreen();

        const textInput = screen.UNSAFE_getByType(PaperTextInput);
        expect(textInput.props.outlineColor).toBe('rgba(216, 133, 133, 1)');
        expect(textInput.props.activeOutlineColor).toBe('rgba(216, 133, 133, 1)');
    });

    it("does not render a list of addresse on invalid input", async () => {
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
        await userAction.type(screen.getByTestId("themedAddressInputTextInput"), 'R u');
        act(() => jest.advanceTimersByTime(1000));

        // Assert
        expect(mockAddressService.fetchAddress).not.toHaveBeenCalled();
        const addressList = screen.queryByTestId("themedAddressInputAddressList");
        expect(addressList).toBeNull();
    });

    it("renders a list of addresses on text input", async () => {
        // Arrange        
        (mockAddressService.fetchAddress as
            jest.MockedFunction<(text: string) => Promise<Address[]>>)
            .mockResolvedValue(mockAddresses);

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
        await userAction.type(screen.getByTestId("themedAddressInputTextInput"), 'Rue de la paix');
        act(() => jest.advanceTimersByTime(1000));

        // Assert
        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalledWith('Rue de la paix');
        });

        const addressList = screen.getByTestId("themedAddressInputAddressList");
        expect(addressList).toBeOnTheScreen();
    });

    it("fills the input field when an address suggestion is clicked", async () => {
        // Arrange        
        (mockAddressService.fetchAddress as
            jest.MockedFunction<(text: string) => Promise<Address[]>>)
            .mockResolvedValue(mockAddresses);

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
        await userAction.type(screen.getByTestId("themedAddressInputTextInput"), 'Rue de la joie');
        act(() => jest.advanceTimersByTime(1000));

        // Assert
        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalledWith('Rue de la joie');
        });

        const addressList = screen.getByTestId("themedAddressInputAddressList");
        expect(addressList).toBeOnTheScreen();

        const addressButton = screen.getAllByTestId("themedAddressInputAddressButton");
        expect(addressButton).toHaveLength(3);

        const address3Text = screen.getByText(mockAddresses[2].fullAddress);
        expect(address3Text).toBeOnTheScreen();

        await userAction.press(address3Text);
        expect(mockField.onChange).toHaveBeenLastCalledWith(mockAddresses[2]);
        expect(mockField.value.city).toBe(mockAddresses[2].city);
        expect(mockField.value.streetInfo).toBe(mockAddresses[2].streetInfo);
    });

    it("hides suggestions when an address suggestion is clicked", async () => {
        // Arrange        
        (mockAddressService.fetchAddress as
            jest.MockedFunction<(text: string) => Promise<Address[]>>)
            .mockResolvedValue(mockAddresses);

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
        await userAction.type(screen.getByTestId("themedAddressInputTextInput"), 'Avenue de la négoce');
        act(() => jest.advanceTimersByTime(1000));
        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalledWith('Avenue de la négoce');
        });

        const address2Text = screen.getByText(mockAddresses[1].fullAddress);
        await userAction.press(address2Text);

        // Assert
        const addressList = screen.queryByTestId("themedAddressInputAddressList");
        expect(addressList).toBeNull();

        const address1Text = screen.queryByText(mockAddresses[0].fullAddress);
        expect(address1Text).toBeNull();
    });

    it("adjust list size to have the same as the textInput", async () => {
        // Arrange        
        (mockAddressService.fetchAddress as
            jest.MockedFunction<(text: string) => Promise<Address[]>>)
            .mockResolvedValue(mockAddresses);

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
        const inputContainer = screen.getByTestId("themedAddressInputContainer");
        act(() => {
            inputContainer.props.onLayout({
                nativeEvent: {
                    layout: { width: 250 }
                }
            });
        });

        await userAction.type(screen.getByTestId("themedAddressInputTextInput"), 'Avenue de la négoce');
        act(() => jest.advanceTimersByTime(1000));
        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalledWith('Avenue de la négoce');
        });

        const addressList = screen.getByTestId('themedAddressInputAddressList');
        const addressListStyle = addressList.props.style;

        expect(addressListStyle).toEqual(
            expect.arrayContaining([
                expect.objectContaining({ width: 250 })
            ])
        );

    });
});