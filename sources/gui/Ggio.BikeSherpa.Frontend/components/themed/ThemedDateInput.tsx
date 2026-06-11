import React from 'react';
import {Control, FieldError, FieldValues, Path, useController} from 'react-hook-form';
import {Text, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {StyleProp, View, ViewStyle} from 'react-native';
import {DatePickerInput} from "react-native-paper-dates";

interface CustomDateInputProps<T extends FieldValues = FieldValues> {
    name: string;
    control: Control<T>;
    label: string;
    validRange?: { startDate: Date | undefined, endDate: Date | undefined, disabledDates: Date[] | undefined }
    error?: FieldError | undefined;
    required?: boolean;
    testID?: string;
    disabled?: boolean;
    style?: StyleProp<ViewStyle>
}

const ThemedDateInput = <T extends FieldValues = FieldValues>(
    {
        name,
        control,
        label,
        validRange,
        error,
        required = false,
        disabled,
        style
    }: CustomDateInputProps<T>
) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name: name as Path<T>
    });

    return (
        <View style={style ? style : formStyle.intputContainer}>
            <Text
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>{label}{required &&
                <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <DatePickerInput
                locale={"fr"}
                inputMode={"start"}
                startWeekOnMonday
                validRange={validRange}
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
                label="Sélectionner une date"
                saveLabel="Confirmer"
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedDateInput;