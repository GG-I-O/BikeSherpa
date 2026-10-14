import React from 'react';
import {Control, FieldError, FieldValues} from 'react-hook-form';
import {IOCContainer} from '@/bootstrapper/constants/IOCContainer';
import {IAddressService} from '@/spi/AddressSPI';
import {ServicesIdentifiers} from '@/bootstrapper/constants/ServicesIdentifiers';
import {ThemedRHFSuggestiveInput} from "@/components/themed/ThemedRHFSuggestiveInput";
import {Address} from "@/models/Address";
import {DimensionValue} from "react-native";

interface ThemedAddressInputProps<T extends FieldValues = FieldValues> {
    name: string;
    control: Control<T>;
    label?: string;
    placeholder?: string;
    error?: FieldError | undefined
    required?: boolean;
    labelAsTitle?: boolean;
    width?: DimensionValue;
}

const ThemedAddressInput = <T extends FieldValues = FieldValues>(
    {
        name,
        control,
        label,
        placeholder,
        error,
        required = false,
        labelAsTitle = false,
        width = undefined
    }: ThemedAddressInputProps<T>
) => {
    const addressService = IOCContainer.get<IAddressService>(ServicesIdentifiers.AddressService);

    return (
        <ThemedRHFSuggestiveInput<T, Address, Address>
            name={name}
            control={control}
            label={label}
            required={required}
            placeholder={placeholder}
            error={error}
            fetchSuggestions={(q) => addressService.fetchAddress(q)}
            getOptionLabel={(a) => (a.name !== "" ? a.name + " " : "") + a.fullAddress}
            getOptionValue={(a) => a}
            getLabelFromValue={a => a.fullAddress}
            labelAsTitle={labelAsTitle}
            width={width}
        />
    );
};

export default ThemedAddressInput;