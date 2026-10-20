import React from 'react';
import {Control, FieldError, FieldValues, Path, useController} from 'react-hook-form';
import {Text, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {StyleProp, View, ViewStyle} from 'react-native';
import {Dropdown} from 'react-native-paper-dropdown';

interface CustomDropdownInputProps<T extends FieldValues = FieldValues> {
    name: string;
    control: Control<T>;
    label: string;
    options: { label: string, value: string }[]
    error?: FieldError | undefined;
    required?: boolean;
    testID?: string;
    isNumber?: boolean;
    style?: StyleProp<ViewStyle>
}

const ThemedDropdownInput = <T extends FieldValues = FieldValues>(
    {
        name,
        control,
        label,
        options,
        error,
        required = false,
        testID,
        isNumber,
        style
    }: CustomDropdownInputProps<T>
) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name: name as Path<T>,
    });

    const handleSelect = (selected?: string) => {
        const next = selected ?? (required ? options[0]?.value : undefined);

        if (next === undefined) {
            field.onChange(isNumber ? 0 : "");
            return;
        }
        field.onChange(isNumber ? parseInt(next) : next);
    };

    return (
        <View style={[formStyle.intputContainer, style]}>
            <Text
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>{label}{required &&
                <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <Dropdown
                value={isNumber ? (field.value ?? "").toString() : field.value ?? ""}
                onSelect={handleSelect}
                mode='outlined'
                error={error !== undefined}
                testID={testID}
                options={options}
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedDropdownInput;