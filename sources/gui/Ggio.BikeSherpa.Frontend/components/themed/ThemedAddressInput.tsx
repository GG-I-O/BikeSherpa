import React, { useCallback, useEffect, useState } from 'react';
import { Control, FieldError, useController } from 'react-hook-form';
import { Button, Text, TextInput, useTheme } from 'react-native-paper';
import formStyle from '@/style/formStyle';
import { FlatList, View } from 'react-native';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { IAddressService } from '@/spi/AddressSPI';
import { ServicesIndentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
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

    function debounce(func: any, delay: number) {
        let timer: number;
        return function (...args: any[]) {
            clearTimeout(timer);
            timer = setTimeout(() => {
                func(...args);
            }, delay);
        };
    }

    const addressService = IOCContainer.get<IAddressService>(ServicesIndentifiers.AddressService);

    const [query, setQuery] = useState<string | null>(null);
    const [debouncedQuery, setDebouncedQuery] = useState<string | null>(null);

    // Debounced function
    const updateQuery = useCallback(
        debounce((input: string) => {
            setDebouncedQuery(input);
        }, 500),
        []
    );

    const onChange = (query: string | null) => {
        setQuery(query);
        updateQuery(query);
    }

    const [suggestedAddresses, setSuggestedAddresses] = useState<Address[] | null>([]);

    const [inputWidth, setInputWidth] = useState<number>(0);

    const onAddressSelect = (address: Address) => {
        onChange(null);
        setSuggestedAddresses(null);
        field.onChange(address);
    }

    useEffect(() => {
        if (!debouncedQuery || debouncedQuery.length < 4) {
            setSuggestedAddresses(null);
            return;
        }

        const fetchAddresses = async (address: string) => {
            const addresses: Address[] | null = await addressService.fetchAddress(address);
            setSuggestedAddresses(addresses);
        }
        fetchAddresses(debouncedQuery);
    }, [debouncedQuery]);

    return (
        <View style={{ width: '80%' }}>
            <Text
                testID='themedAddressInputLabel'
                style={[formStyle.label, { color: theme.colors.onBackground }, theme.fonts.labelLarge]}>
                {label}
                {required && <Text style={{ color: theme.colors.error }}> *</Text>}
            </Text>
            <View onLayout={(event) => setInputWidth(event.nativeEvent.layout.width)}>
                <TextInput
                    testID='themedAddressInputTextInput'
                    value={field.value.name ?? ''}
                    onChangeText={(text: string) => {
                        field.onChange({ name: text });
                        onChange(text);
                    }}
                    placeholder={placeholder}
                    placeholderTextColor={placeholderTextColor || '#3636367e'}
                    secureTextEntry={secureTextEntry}
                    mode='outlined'
                    style={[formStyle.input,
                    error && { borderColor: theme.colors.error },
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
                    keyExtractor={item => item.name}
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
            onPress={() => onPress(address)}
            style={formStyle.addressOptionElement}
        >
            <Text style={{ width: '100%', textAlign: 'left' }}>{address.name}</Text>
        </Button>
    );
}