import React from 'react';
import {Control, FieldError} from 'react-hook-form';
import {IOCContainer} from '@/bootstrapper/constants/IOCContainer';
import {IAddressService} from '@/spi/AddressSPI';
import {ServicesIdentifiers} from '@/bootstrapper/constants/ServicesIdentifiers';
import {ThemedRHFSuggestiveInput} from "@/components/themed/ThemedRHFSuggestiveInput";

interface ThemedAddressInputProps {
    name: string;
    control: Control<any>;
    label?: string;
    placeholder?: string;
    error?: FieldError | undefined
    required?: boolean;
}

const ThemedAddressInput: React.FC<ThemedAddressInputProps> = (
    {
        name,
        control,
        label,
        placeholder,
        error,
        required = false
    }
) => {
    const addressService = IOCContainer.get<IAddressService>(ServicesIdentifiers.AddressService);

    return (
        <ThemedRHFSuggestiveInput
            name={name}
            control={control}
            label={label}
            required={required}
            placeholder={placeholder}
            error={error}
            fetchSuggestions={(q) => addressService.fetchAddress(q)}
            renderLabel={(a) => a.fullAddress}
        />
    );
};

export default ThemedAddressInput;