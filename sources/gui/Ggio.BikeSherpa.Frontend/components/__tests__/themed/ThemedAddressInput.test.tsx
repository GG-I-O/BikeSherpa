import * as ReactHookForm from 'react-hook-form';
import {act, render, screen, userEvent, waitFor} from '@testing-library/react-native';
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {Address} from "@/models/Address";
import {useTheme} from 'react-native-paper';
import {faker} from '@faker-js/faker';
import {createRandomAddress} from '@/fixtures/address-fixtures';
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";

const mockAddresses: Address[] = faker.helpers.multiple(
    () => createRandomAddress(faker.company.name()),
    {count: 3}
);

const mockField = {
    name: 'address',
    value: null as Address | null,
    onChange: jest.fn((newValue) => {
        mockField.value = newValue;
    }),
    onBlur: jest.fn(),
    ref: jest.fn()
};

const mockAddressService = {
    fetchAddress: jest.fn()
};

jest.mock('react-native-safe-area-context', () => ({
    useSafeAreaInsets: () => ({top: 0, bottom: 0, left: 0, right: 0})
}));

jest.mock('react-hook-form', () => ({
    ...jest.requireActual('react-hook-form'),
    useController: jest.fn()
}));

jest.mock("@/bootstrapper/constants/IOCContainer");

jest.mock('react-native-paper', () => {
    const actual = jest.requireActual('react-native-paper');
    return {
        ...actual,
        useTheme: jest.fn(),
        Portal: ({children}: any) => children
    };
});

describe("ThemedAddressInput (integration)", () => {
    let user: ReturnType<typeof userEvent.setup>;
    const mockUseTheme = useTheme as jest.Mock;

    beforeEach(() => {
        jest.clearAllMocks();
        jest.useFakeTimers();

        user = userEvent.setup({
            advanceTimers: jest.advanceTimersByTime
        });

        mockUseTheme.mockReturnValue({
            colors: {
                background: 'white',
                onBackground: 'black',
                error: 'red',
                outline: 'gray'
            },
            fonts: {}
        });

        mockField.value = null;

        (ReactHookForm.useController as jest.Mock).mockReturnValue({
            field: mockField,
            fieldState: {},
            formState: {}
        });

        (IOCContainer.get as jest.Mock).mockReturnValue(mockAddressService);
    });

    afterEach(() => {
        act(() => jest.runOnlyPendingTimers());
        jest.useRealTimers();
    });

    const flushAsync = async () => {
        await act(async () => {
            jest.advanceTimersByTime(500);
        });

        await act(async () => {
        });
    };

    it("renders label, placeholder and required star", () => {
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="Adresse"
                placeholder="10 rue"
                required
            />
        );

        const label = screen.getByTestId("themedSuggestiveInputLabel");
        expect(label).toBeOnTheScreen();
        expect(screen.getByText("*")).toBeOnTheScreen();
        expect(screen.getByTestId("themedSuggestiveTextInput").props.placeholder).toBe("10 rue");
    });

    it("does not render required star when not required", () => {
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="Adresse"
            />
        );

        expect(screen.queryByText("*")).toBeNull();
    });

    it("renders error message", () => {
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
                label="Adresse"
                error={{type: 'required', message: 'Erreur Adresse'}}
            />
        );

        expect(screen.getByText("Erreur Adresse")).toBeOnTheScreen();
    });

    it("does not fetch suggestions if input is too short", async () => {
        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
            />
        );

        await user.type(
            screen.getByTestId("themedSuggestiveTextInput"),
            "Ru"
        );

        await act(async () => {
            jest.advanceTimersByTime(500);
        });


        expect(mockAddressService.fetchAddress).not.toHaveBeenCalled();
        expect(screen.queryByTestId("themedSuggestiveInputSuggestionsList")).toBeNull();
    });

    it("fetches and displays suggestions", async () => {
        mockAddressService.fetchAddress.mockResolvedValue(mockAddresses);

        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
            />
        );

        await user.type(
            screen.getByTestId("themedSuggestiveTextInput"),
            "Rue de Paris"
        );

        await flushAsync();

        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalledWith("Rue de Paris");
        });

        expect(
            screen.getByTestId("themedSuggestiveInputSuggestionsList")
        ).toBeOnTheScreen();
    });

    it("selects a suggestion and updates form value", async () => {
        mockAddressService.fetchAddress.mockResolvedValue(mockAddresses);

        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
            />
        );

        await user.type(
            screen.getByTestId("themedSuggestiveTextInput"),
            "Rue test"
        );

        await flushAsync();

        await waitFor(() => {
            expect(mockAddressService.fetchAddress).toHaveBeenCalled();
        });

        const itemText = await screen.findByText(mockAddresses[1].fullAddress);

        await user.press(itemText);

        expect(mockField.onChange).toHaveBeenCalledWith(mockAddresses[1]);
    });

    it("closes suggestions after selection", async () => {
        mockAddressService.fetchAddress.mockResolvedValue(mockAddresses);

        render(
            <ThemedAddressInput
                name="address"
                control={{} as any}
            />
        );

        await user.type(
            screen.getByTestId("themedSuggestiveTextInput"),
            "Rue fermeture"
        );

        await flushAsync();

        const itemText = await screen.findByText(mockAddresses[0].fullAddress);

        await user.press(itemText);

        await waitFor(() => {
            expect(
                screen.queryByTestId("themedSuggestiveInputSuggestionsList")
            ).toBeNull();
        });
    });
});