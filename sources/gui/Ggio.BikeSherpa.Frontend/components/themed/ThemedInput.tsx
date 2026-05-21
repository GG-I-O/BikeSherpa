import React from 'react';
import {Control, FieldError, useController} from 'react-hook-form';
import {Text, TextInput, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {View} from 'react-native';

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
    disabled?: boolean;
    isAnArray?: boolean;
    isNumeric?: boolean;
}

const ThemedInput: React.FC<CustomTextInputProps> = (
    {
        name,
        control,
        label,
        placeholder,
        error,
        secureTextEntry,
        placeholderTextColor,
        required = false,
        testID,
        disabled,
        isAnArray,
        isNumeric
    }) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name,
    });
    
    const onChange = (value: string) => {
        if (isNumeric) {
            const cleaned = value.replace(/[^0-9.]/g, '');
            if (cleaned === '' || cleaned === '.') {
                field.onChange(0);
            } else {
                const parsed = parseFloat(cleaned);
                field.onChange(isNaN(parsed) ? 0 : parsed);
            }
        }
        else if (isAnArray)
            field.onChange([value]);
        else 
            field.onChange(value);
    }

    return (
        <View style={formStyle.intputContainer}>
            <Text
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>{label}{required &&
                <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <TextInput
                value={ isAnArray ? field.value[0] : field.value}
                onChangeText={(value) => onChange(value)}
                placeholder={placeholder}
                placeholderTextColor={placeholderTextColor || '#3636367e'}
                secureTextEntry={secureTextEntry}
                mode='outlined'
                style={[formStyle.input,
                    error && {borderColor: theme.colors.error},
                    {
                        backgroundColor: theme.colors.background,
                        color: theme.colors.onBackground,
                    }
                ]}
                contentStyle={{color: theme.colors.onBackground}}
                testID={testID}
                disabled={disabled}
                keyboardType={isNumeric ? "numeric" : "default"}
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedInput;