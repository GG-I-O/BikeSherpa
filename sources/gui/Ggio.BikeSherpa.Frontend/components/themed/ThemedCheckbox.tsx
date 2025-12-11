import React from 'react';
import { Control, FieldError, useController } from 'react-hook-form';
import { useThemeColors } from '@/hooks/useThemeColors';
import { Checkbox, Text } from 'react-native-paper';
import formStyle from '@/style/formStyle';
import { View } from 'react-native';

interface CustomCheckboxProps {
    name: string;
    control: Control<any>;
    label: string;
    error?: FieldError | undefined
}
const ThemedCheckbox: React.FC<CustomCheckboxProps> = ({
    name,
    control,
    label,
    error,
}) => {
    const colors = useThemeColors();

    const { field } = useController({
        control,
        name,
    });

    return (
        <View style={formStyle.checkboxContainer}>
            <Checkbox
                status={field.value ? 'checked' : 'unchecked'}
                onPress={() => field.onChange(!field.value)}
            />
            <Text>{label}</Text>
            {error && (<Text style={{ color: colors.error }}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedCheckbox;