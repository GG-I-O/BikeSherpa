import React from 'react';
import { Control, FieldError, useController } from 'react-hook-form';
import { Text, TextInput, useTheme } from 'react-native-paper';
import formStyle from '@/style/formStyle';
import { View } from 'react-native';

interface CustomTextInputProps {
    name: string;
    control: Control<any>;
    label: string;
    placeholder: string;
    error?: FieldError | undefined;
    secureTextEntry?: boolean;
    placeholderTextColor?: string;
    required?: boolean;
    testID?: string;
}
const ThemedInput: React.FC<CustomTextInputProps> = ({
    name,
    control,
    label,
    placeholder,
    error,
    secureTextEntry,
    placeholderTextColor,
    required = false,
    testID
}) => {
    const theme = useTheme();

    const { field } = useController({
        control,
        name,
    });

    return (
        <View style={formStyle.intputContainer}>
            <Text style={[formStyle.label, { color: theme.colors.onBackground }, theme.fonts.labelLarge]}>{label}{required && <Text style={{ color: theme.colors.error }}> *</Text>}
            </Text>
            <TextInput
                value={field.value}
                onChangeText={field.onChange}
                placeholder={placeholder}
                placeholderTextColor={placeholderTextColor || '#3636367e'}
                secureTextEntry={secureTextEntry}
                mode='outlined'
                style={[formStyle.input,
                error && { borderColor: theme.colors.error },
                {
                    backgroundColor: theme.colors.background,
                    color: theme.colors.onBackground,
                }
                ]}
                contentStyle={{ color: theme.colors.onBackground }}
                testID={testID}
            />
            {error && (<Text style={{ color: theme.colors.error }}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedInput;