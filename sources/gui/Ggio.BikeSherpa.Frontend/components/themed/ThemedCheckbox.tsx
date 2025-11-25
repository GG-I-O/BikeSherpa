import React from 'react';
import { Control, FieldError, useController } from 'react-hook-form';
import { useThemeColors } from '@/hooks/useThemeColors';
import { Checkbox, Provider, Text, TextInput } from 'react-native-paper';

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
        <Provider>
            <Text style={{ textAlign: 'center' }}>{label}</Text>
            <Checkbox
                status={field.value == true ? 'checked' : 'unchecked'}
                onPress={field.onChange}
            />
            {error && (<Text style={{ color: colors.error }}>{error.message}</Text>)}
        </Provider>
    );
};

export default ThemedCheckbox;