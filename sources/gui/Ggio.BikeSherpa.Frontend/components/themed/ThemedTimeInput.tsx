import React from 'react';
import {Control, FieldError, useController} from 'react-hook-form';
import {Text, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {View} from 'react-native';
import TimePickerInput from "@/components/general/TimePickerInput";

interface CustomTimeInputProps {
    name: string;
    control: Control<any>;
    label: string;
    error?: FieldError | undefined;
    required?: boolean;
    testID?: string;
    disabled?: boolean;
}

const ThemedTimeInput: React.FC<CustomTimeInputProps> = (
    {
        name,
        control,
        label,
        error,
        required = false
    }) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name,
    });

    const fieldDate = field.value ? new Date(field.value) : new Date();

    return (
        <View style={formStyle.intputContainer}>
            <Text
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>{label}{required &&
                <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <TimePickerInput
                hours={fieldDate.getHours()}
                minutes={fieldDate.getMinutes()}
                onConfirm={({hours, minutes}: { hours: number; minutes: number; }): void => {
                    let newDate: Date = new Date(field.value);
                    newDate.setHours(hours);
                    newDate.setMinutes(minutes);
                    field.onChange(newDate.toISOString());
                }}
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedTimeInput;