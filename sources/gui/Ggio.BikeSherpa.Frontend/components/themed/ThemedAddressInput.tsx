import React, { useCallback, useEffect, useState } from 'react';
import { Control, FieldError, useController } from 'react-hook-form';
import { Button, Text, TextInput, useTheme } from 'react-native-paper';
import formStyle from '@/style/formStyle';
import { FlatList, View } from 'react-native';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { IAddressService } from '@/spi/AddressSPI';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { Address } from '@/models/Address';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

interface ThemedAddressInputProps {
    name: string;
    control: Control<any>;
    label: string;
    placeholder: string;
    error?: FieldError | undefined
    secureTextEntry?: boolean;
    placeholderTextColor?: string;
    required?: boolean;
}
const ThemedAddressInput: React.FC<ThemedAddressInputProps> = ({
    name,
    control,
    label,
    placeholder,
    error,
    secureTextEntry,
    placeholderTextColor,
    required = false
}) => {
    const theme = useTheme();
    const safeInset = useSafeAreaInsets();

    const { field } = useController({
        control,
        name,
    });

    const addressService = IOCContainer.get<IAddressService>(ServicesIdentifiers.AddressService);

    const [query, setQuery] = useState<string | null>(null);
    const [debouncedQuery, setDebouncedQuery] = useState<string | null>(null);
    const [suggestedAddresses, setSuggestedAddresses] = useState<Address[] | null>([]);
    const [inputWidth, setInputWidth] = useState<number>(0);
    const [timer, setTimer] = useState<number>();

    const updateQueryWithDebounce = useCallback((query: string | null) => {
        clearTimeout(timer);
        const timeout = setTimeout(() => {
            setDebouncedQuery(query);
        }, 500);
        setTimer(timeout);
    }, [setDebouncedQuery, timer, setTimer]);

    const onTextInputChange = (query: string | null) => {
        setQuery(query);
        updateQueryWithDebounce(query);
    }

    const onAddressSelect = (address: Address) => {
        onTextInputChange(null);
        setSuggestedAddresses(null);
        // address is coming from the API and does not contain a fulladdress property
        const newAddress = address;
        newAddress.fullAddress = address.name;
        field.onChange(newAddress);
    }

    useEffect(() => {
        if (!debouncedQuery || !query || debouncedQuery.length < 4) {
            setSuggestedAddresses(null);
            return;
        }

        const fetchAddresses = async (address: string) => {
            const addresses: Address[] | null = await addressService.fetchAddress(address);
            setSuggestedAddresses(addresses);
        }
        fetchAddresses(debouncedQuery)
            .then();
    }, [debouncedQuery, query, addressService]);

    return (
        <View style={{ width: '80%' }}>
            <Text
                testID='themedAddressInputLabel'
                style={[formStyle.label, { color: theme.colors.onBackground }, theme.fonts.labelLarge]}>
                {label}
                {required && <Text style={{ color: theme.colors.error }}> *</Text>}
            </Text>
            <View
                testID='themedAddressInputContainer'
                onLayout={(event) => setInputWidth(event.nativeEvent.layout.width)}
            >
                <TextInput
                    testID='themedAddressInputTextInput'
                    value={field.value?.fullAddress || ''}
                    onChangeText={(text: string) => {
                        field.onChange({ fullAddress: text });
                        onTextInputChange(text);
                    }}
                    placeholder={placeholder}
                    placeholderTextColor={placeholderTextColor || '#3636367e'}
                    secureTextEntry={secureTextEntry}
                    mode='outlined'
                    outlineColor={error ? theme.colors.error : undefined}
                    activeOutlineColor={error ? theme.colors.error : undefined}
                    style={[formStyle.input,
                    {
                        backgroundColor: theme.colors.background,
                        color: theme.colors.onBackground
                    }
                    ]}
                    contentStyle={{ color: theme.colors.onBackground }}
                />
            </View>

            {error && (<Text style={{ color: theme.colors.error }}>{error.message}</Text>)}

            {(suggestedAddresses != null && suggestedAddresses.length > 0) &&
                <FlatList
                    testID='themedAddressInputAddressList'
                    ListFooterComponent={
                        <View style={{ height: safeInset.bottom * 2 }} />
                    }
                    data={suggestedAddresses}
                    keyExtractor={item => item.fullAddress}
                    style={[formStyle.addressOptionList, {
                        borderColor: theme.colors.primaryContainer,
                        backgroundColor: theme.colors.background,
                        width: inputWidth
                    }]}
                    contentContainerStyle={formStyle.addressOptionContainer}
                    CellRendererComponent={({ children, style, ...props }) => (
                        <View style={[style, { width: '100%' }]} {...props}>
                            {children}
                        </View>
                    )}
                    renderItem={({ item }) => <AddressOption address={item} onPress={onAddressSelect} />}
                />
            }

        </View>
    );
};

export default ThemedAddressInput;

// -----------------------------------------
type AddressOptionProps = {
    address: Address,
    onPress: (address: Address) => void
}

function AddressOption({ address, onPress }: AddressOptionProps) {
    return (
        <Button
            testID={`themedAddressInputAddressButton`}
            onPress={() => onPress(address)}
            style={formStyle.addressOptionElement}
        >
            <Text style={{ width: '100%', textAlign: 'left' }}>{address.name}</Text>
        </Button>
    );
}