import React from 'react';
import {Control, FieldError, useController} from 'react-hook-form';
import {Text, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {View} from 'react-native';
import {DatePickerInput} from "react-native-paper-dates";

interface CustomDateInputProps {
    name: string;
    control: Control<any>;
    label: string;
    error?: FieldError | undefined;
    required?: boolean;
    testID?: string;
    disabled?: boolean;
}

const ThemedDateInput: React.FC<CustomDateInputProps> = (
    {
        name,
        control,
        label,
        error,
        required = false,
        disabled
    }) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name,
    });

    return (
        <View style={formStyle.intputContainer}>
            <Text
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>{label}{required &&
                <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <DatePickerInput
                locale={"fr"}
                inputMode={"start"}
                onChange={(date: Date | undefined): void => {
                    if (!date) return;

                    const baseDate = field.value ? new Date(field.value) : new Date();

                    const newDate = new Date(date);
                    newDate.setHours(baseDate.getHours());
                    newDate.setMinutes(baseDate.getMinutes());

                    field.onChange(newDate.toISOString());
                }}
                value={field.value ? new Date(field.value) : undefined}
                disabled={disabled}
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedDateInput;