import React from 'react';
import { Control, FieldError, useController } from 'react-hook-form';
import { useThemeColors } from '@/hooks/useThemeColors';
import { Provider, Text, TextInput } from 'react-native-paper';

interface CustomTextInputProps {
    name: string;
    control: Control<any>;
    placeholder: string;
    error?: FieldError | undefined
    secureTextEntry?: boolean;
    placeholderTextColor?: string;
}
const ThemedInput: React.FC<CustomTextInputProps> = ({
    name,
    control,
    placeholder,
    error,
    secureTextEntry,
    placeholderTextColor,
}) => {
    const colors = useThemeColors();

    const { field } = useController({
        control,
        name,
    });

    return (
        <Provider>
            <Text style={{ textAlign: 'center' }}>{placeholder}</Text>
            <TextInput
                value={field.value}
                onChangeText={field.onChange}
                placeholder={placeholder}
                placeholderTextColor={placeholderTextColor || '#3636367e'}
                secureTextEntry={secureTextEntry}
                mode='outlined'
                style={[
                    error && { borderColor: colors.error },
                    {
                        padding: 8,
                        borderRadius: 10,
                        borderWidth: 1,
                        borderColor: colors.tint,
                        backgroundColor: 'white',
                        color: 'black',
                    }
                ]}
            />
            {error && (<Text style={{ color: colors.error }}>{error.message}</Text>)}
        </Provider>
    );
};

export default ThemedInput;