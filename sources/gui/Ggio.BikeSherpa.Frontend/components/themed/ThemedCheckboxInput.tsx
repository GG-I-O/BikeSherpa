import React from 'react';
import {Control, FieldError, useController} from 'react-hook-form';
import {Checkbox, Text, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {View} from 'react-native';

interface CustomCheckboxInputProps {
    name: string;
    control: Control<any>;
    label: string;
    error?: FieldError | undefined;
    required?: boolean;
    testID?: string;
}

const ThemedCheckboxInput: React.FC<CustomCheckboxInputProps> = (
    {
        name,
        control,
        label,
        error,
        required = false,
        testID
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
            <Checkbox
                status={field.value ? 'checked' : 'unchecked'}
                onPress={() => field.onChange(!field.value)}
                testID={testID}
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedCheckboxInput;