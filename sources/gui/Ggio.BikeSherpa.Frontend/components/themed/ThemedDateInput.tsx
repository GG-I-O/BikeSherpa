import React, {useState} from 'react';
import {Control, FieldError, useController} from 'react-hook-form';
import {Text, TextInput, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {View} from 'react-native';

interface CustomDateInputProps {
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
}

const ThemedDateInput: React.FC<CustomDateInputProps> = (
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
        disabled
    }) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name,
    });

    const date = new Date(field.value);
    const inputText = `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
    const [textValue, setTextValue] = useState(inputText);

    const onChange = (value: string) => {
        const parsedString = value.split('/');
        
        const parsedNumber = parsedString.map((text) => {
            if (text === "")
                return 1;
            return parseInt(text)
        });
        
        const newDate = new Date(Date.UTC(
            parsedNumber[2],
            parsedNumber[1] - 1,
            parsedNumber[0]
        ));
        field.onChange(newDate.toISOString());
        
        const inputText = `${newDate.getDate()}/${newDate.getMonth() + 1}/${newDate.getFullYear()}`;
        setTextValue(inputText);
    }

    return (
        <View style={formStyle.intputContainer}>
            <Text
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>{label}{required &&
                <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <TextInput
                value={textValue}
                onChangeText={(value) => {
                    setTextValue(value);
                }}
                onBlur={() => {
                    onChange(textValue);
                }}
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
            />
            {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
        </View>
    );
};

export default ThemedDateInput;