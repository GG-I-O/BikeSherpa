import React, {useMemo, useState} from "react";
import {LayoutChangeEvent, StyleSheet, View} from "react-native";
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

const GAP = 8;
const MIN_TILE_WIDTH = 170;

const computeColumns = (count: number, containerWidth: number): number => {
    if (count <= 0) return 1;

    const maxColumns = Math.min(
        count,
        Math.max(1, Math.floor((containerWidth + GAP) / (MIN_TILE_WIDTH + GAP)))
    );

    const rows = Math.ceil(count / maxColumns);
    const balanced = Math.ceil(count / rows);

    for (let c = maxColumns; c >= balanced; c--) {
        if (count % c === 0) return c;
    }
    return balanced;
};

const chunk = <T, >(items: T[], size: number): T[][] => {
    const result: T[][] = [];
    for (let i = 0; i < items.length; i += size) {
        result.push(items.slice(i, i + size));
    }
    return result;
};

const ThemedSegmentedInput = <T extends FieldValues = FieldValues>(
    {name, control, buttons, label, required = false, error}: Props<T>) => {
    const theme = useTheme();
    const {field} = useController({control, name: name as Path<T>});
    const [containerWidth, setContainerWidth] = useState(0);

    const columns = computeColumns(buttons.length, containerWidth);
    const rows = useMemo(() => chunk(buttons, columns), [buttons, columns]);

    const onLayout = (e: LayoutChangeEvent) => {
        const width = Math.round(e.nativeEvent.layout.width);
        setContainerWidth((previous) => (previous === width ? previous : width));
    };

    return (
        <View style={{gap: 8, width: "100%"}}>
            {!!label && (
                <Text style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>
                    {label}
                    {required && <Text style={{color: theme.colors.error}}> *</Text>}
                </Text>
            )}

            <View onLayout={onLayout} style={styles.measure}>
                <View accessibilityRole="radiogroup" style={styles.group}>
                    {rows.map((row, rowIndex) => (
                        <View key={rowIndex} style={styles.row}>
                            {row.map((button) => {
                                const selected = field.value === button.value;
                                const textColor = selected
                                    ? theme.colors.onSecondaryContainer
                                    : theme.colors.onSurface;
                                const borderColor = error
                                    ? theme.colors.error
                                    : selected ? theme.colors.secondary : theme.colors.outline;

                                return (
                                    <View key={button.value} style={styles.cell}>
                                        <TouchableRipple
                                            testID={button.testID}
                                            disabled={button.disabled}
                                            onPress={() => field.onChange(button.value)}
                                            accessibilityRole="radio"
                                            accessibilityState={{selected, disabled: button.disabled}}
                                            style={[
                                                styles.item,
                                                {borderColor},
                                                selected && {backgroundColor: theme.colors.secondaryContainer},
                                                button.disabled && {opacity: 0.4}
                                            ]}
                                        >
                                            <View style={styles.itemContent}>
                                                <Icon
                                                    source={selected ? "radiobox-marked" : "radiobox-blank"}
                                                    size={20}
                                                    color={selected ? theme.colors.primary : theme.colors.onSurfaceVariant}
                                                />
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
                                    </View>
                                );
                            })}
                        </View>
                    ))}
                </View>
            </View>

            {error && <Text style={{color: theme.colors.error}}>{error.message}</Text>}
        </View>
    );
};

const styles = StyleSheet.create({
    measure: {
        width: "100%",
        minWidth: 0
    },
    group: {
        gap: GAP
    },
    row: {
        flexDirection: "row",
        gap: GAP
    },
    cell: {
        flex: 1,
        flexBasis: 0,
        minWidth: 0
    },
    item: {
        flex: 1,
        paddingVertical: 6,
        paddingHorizontal: 10,
        justifyContent: "center",
        borderWidth: 1,
        borderRadius: 12,
        overflow: "hidden"
    },
    itemContent: {
        flexDirection: "row",
        alignItems: "center",
        gap: 8
    },
    itemLabel: {
        flex: 1,
        flexShrink: 1
    },
});

export default ThemedSegmentedInput;