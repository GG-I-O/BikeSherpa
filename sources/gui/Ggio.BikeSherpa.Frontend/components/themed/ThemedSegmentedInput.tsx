import React from "react";
import {StyleSheet, View} from "react-native";
import {Control, FieldError, FieldValues, Path, useController} from "react-hook-form";
import {Icon, Text, TouchableRipple, useTheme} from "react-native-paper";
import formStyle from "@/style/formStyle";

type SegmentedOption = {
    value: string;
    label: string;
    icon?: string;
    disabled?: boolean;
    testID?: string;
};

interface Props<T extends FieldValues = FieldValues> {
    name: string;
    control: Control<T>;
    buttons: SegmentedOption[];
    label?: string;
    required?: boolean;
    error?: FieldError | undefined;
}

const ThemedSegmentedInput = <T extends FieldValues = FieldValues>(
    {name, control, buttons, label, required = false, error}: Props<T>) => {
    const theme = useTheme();
    const {field} = useController({control, name: name as Path<T>});

    return (
        <View style={{gap: 8, width: "100%"}}>
            {!!label && (
                <Text style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>
                    {label}
                    {required && <Text style={{color: theme.colors.error}}> *</Text>}
                </Text>
            )}

            <View
                accessibilityRole="radiogroup"
                style={[
                    styles.group,
                    {borderColor: error ? theme.colors.error : theme.colors.outline}
                ]}
            >
                {buttons.map((button, i) => {
                    const selected = field.value === button.value;
                    const textColor = selected
                        ? theme.colors.onSecondaryContainer
                        : theme.colors.onSurface;

                    return (
                        <TouchableRipple
                            key={button.value}
                            testID={button.testID}
                            disabled={button.disabled}
                            onPress={() => field.onChange(button.value)}
                            accessibilityRole="radio"
                            accessibilityState={{selected, disabled: button.disabled}}
                            style={[
                                styles.item,
                                i > 0 && {borderLeftWidth: StyleSheet.hairlineWidth * 2, borderLeftColor: theme.colors.outline},
                                selected && {backgroundColor: theme.colors.secondaryContainer},
                                button.disabled && {opacity: 0.4}
                            ]}
                        >
                            <View style={styles.itemContent}>
                                {button.icon && <Icon source={button.icon} size={18} color={textColor}/>}
                                <Text
                                    style={[
                                        theme.fonts.labelLarge,
                                        styles.itemLabel,
                                        {color: textColor, fontWeight: selected ? "700" : "500"}
                                    ]}
                                >
                                    {button.label}
                                </Text>
                            </View>
                        </TouchableRipple>
                    );
                })}
            </View>

            {error && <Text style={{color: theme.colors.error}}>{error.message}</Text>}
        </View>
    );
};

const styles = StyleSheet.create({
    group: {
        flexDirection: "row",
        borderWidth: 1,
        borderRadius: 20,
        overflow: "hidden"
    },
    item: {
        flex: 1,
        minHeight: 52,
        paddingVertical: 8,
        paddingHorizontal: 6,
        justifyContent: "center"
    },
    itemContent: {
        alignItems: "center",
        justifyContent: "center",
        gap: 2
    },
    itemLabel: {
        textAlign: "center",
        flexShrink: 1
    },
});

export default ThemedSegmentedInput;