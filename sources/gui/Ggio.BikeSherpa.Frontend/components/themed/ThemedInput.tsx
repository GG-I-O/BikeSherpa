import React from 'react';
import {Control, FieldError, FieldValues, Path, useController} from 'react-hook-form';
import {Text, TextInput, useTheme} from 'react-native-paper';
import formStyle from '@/style/formStyle';
import {DimensionValue, View} from 'react-native';
import {Icon} from "react-native-paper/src";
import {useInputGroup} from "@/components/themed/InputGroup";

interface CustomTextInputProps<T extends FieldValues = FieldValues> {
    name: string;
    control: Control<T>;
    label: string;
    labelIconCode?: string;
    horizontal?: boolean;
    placeholder: string;
    error?: FieldError | undefined;
    secureTextEntry?: boolean;
    placeholderTextColor?: string;
    required?: boolean;
    testID?: string;
    disabled?: boolean;
    isAnArray?: boolean;
    isNumeric?: boolean;
    width?: DimensionValue;
}

const ThemedInput = <T extends FieldValues = FieldValues>(
    {
        name,
        control,
        label,
        labelIconCode = undefined,
        horizontal = false,
        placeholder,
        error,
        secureTextEntry,
        placeholderTextColor,
        required = false,
        testID,
        disabled,
        isAnArray,
        isNumeric,
        width = undefined
    }: CustomTextInputProps<T>) => {
    const theme = useTheme();
    const group = useInputGroup();
    const alignLabel = horizontal && !!group;

    const {field} = useController({
        control,
        name: name as Path<T>
    });

    const onChange = (value: string) => {
        if (isNumeric) {
            const cleaned = value.replace(/[^0-9.]/g, '');

            if (cleaned === '' || cleaned === '.') {
                field.onChange('');
                return;
            }

            if (cleaned.endsWith('.')) {
                field.onChange(cleaned);
                return;
            }

            const parsed = parseFloat(cleaned);
            field.onChange(isNaN(parsed) ? '' : parsed);
        } else if (isAnArray)
            field.onChange([value]);
        else
            field.onChange(value);
    }

    return (
        <View style={[
            formStyle.intputContainer,
            horizontal
                ? {flexDirection: 'row', gap: 8, alignItems: 'center', width: '100%'}
                : {flexDirection: 'column'},
            width ? {width: width} : {}
        ]}>
            <View style={[
                horizontal && {flexShrink: 0, minHeight: 40, justifyContent: 'center'},
                alignLabel && group.labelWidth > 0 ? {width: group.labelWidth} : null
            ]}>
                <View
                    style={{alignSelf: 'flex-start', flexDirection: 'row', alignItems: 'center', gap: 4}}
                    onLayout={alignLabel ? (e) => group.reportLabelWidth(e.nativeEvent.layout.width) : undefined}
                >
                    {labelIconCode && (
                        <Icon size={16} source={labelIconCode} color={theme.colors.primary}/>
                    )}
                    <Text
                        style={[
                            formStyle.label,
                            {color: theme.colors.onBackground, flexShrink: 1},
                            theme.fonts.labelLarge
                        ]}
                    >
                        {label}
                        {required && <Text style={{color: theme.colors.error}}> *</Text>}
                    </Text>
                </View>
            </View>
            <View style={horizontal ? {flex: 1, minWidth: 0} : undefined}>
                <TextInput
                    value={(isAnArray ? field.value?.[0] : field.value) ?? ''}
                    onChangeText={(value) => onChange(value)}
                    placeholder={placeholder}
                    placeholderTextColor={placeholderTextColor || theme.colors.outline}
                    secureTextEntry={secureTextEntry}
                    mode='outlined'
                    style={[formStyle.input,
                        error && {borderColor: theme.colors.error},
                        {
                            backgroundColor: theme.colors.background,
                            color: theme.colors.onBackground
                        }
                    ]}
                    contentStyle={{color: theme.colors.onBackground}}
                    testID={testID}
                    disabled={disabled}
                    keyboardType={isNumeric ? "numeric" : "default"}
                />
                {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
            </View>
        </View>
    );
};

export default ThemedInput;