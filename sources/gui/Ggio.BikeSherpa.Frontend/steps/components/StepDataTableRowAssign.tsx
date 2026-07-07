import {Button, DataTable, IconButton, Text, TextInput, useTheme} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import React, {useCallback, useState} from "react";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import TimePickerInput from "@/components/general/TimePickerInput";
import {View} from "react-native";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import useStepDataTableRowViewModel from "@/steps/viewModel/useStepDataTableRowViewModel";
import {Icon} from "react-native-paper/src";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IColorServiceSpi} from "@/spi/ColorServiceSpi";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {DatePickerModal, TimePickerModal} from "react-native-paper-dates";
import {SafeAreaProvider} from "react-native-safe-area-context";

type Props = {
    step: StepToDisplay,
    isSelected?: boolean,
    onPress?: (step: StepToDisplay) => void,
    canChangeDate?: boolean,
    isFirst: boolean,
    isLast: boolean
}

export default function StepDataTableRowAssign(
    {
        step,
        isSelected = false,
        onPress,
        canChangeDate = false,
        isFirst = false,
        isLast = false
    }: Readonly<Props>) {
    const theme = useTheme();
    const style = datatableStyle;

    const [isTimePickerOpen, setIsTimePickerOpen] = useState(false); // Disable onRowPress if we're picking time
    const [isCompletionDatePickerOpen, setIsCompletionDatePickerOpen] = useState(false);
    const [isCompletionTimePickerOpen, setIsCompletionTimePickerOpen] = useState(false);
    const [completionDate, setCompletionDate] = useState<Date | undefined>(new Date());

    const viewModel = useStepDataTableRowViewModel(step);
    const colorService = IOCContainer.get<IColorServiceSpi>(ServicesIdentifiers.ColorService);

    const getBackgroundColor = () => {
        if (isSelected) return theme.colors.primary;
        if (!step.deliveryCode) return theme.colors.background;

        const color = colorService.stringToColor(step.deliveryCode);
        return color + '20';
    };

    const onConfirmDatePicker = useCallback((params: { date: Date | undefined }) => {
        setIsCompletionDatePickerOpen(false);
        if (params.date) {
            setCompletionDate(params.date);
            setIsCompletionTimePickerOpen(true);
        }
    }, []);

    const onConfirmTimePicker = useCallback(({hours, minutes}: { hours: number, minutes: number }) => {
        setIsCompletionTimePickerOpen(false);
        const finalDate = completionDate ? new Date(completionDate) : new Date();
        finalDate.setHours(hours);
        finalDate.setMinutes(minutes);
        viewModel.completeStep(step.id, finalDate);
    }, [completionDate, step.id, viewModel]);

    return (
        <SafeAreaProvider>
            <DataTable.Row
            onPress={() => {
                if (isTimePickerOpen) return;
                if (onPress) onPress(step);
            }}
            style={{backgroundColor: getBackgroundColor()}}
        >
            <DataTable.Cell style={[style.column, style.width40]}>
                <View style={{flexDirection: "column", gap: 0}}>
                    <IconButton
                        style={{margin: 0}}
                        icon="arrow-up-bold"
                        onPress={() => viewModel.reorderStepForADay(step.id, -1)}
                        disabled={isFirst}
                    />
                    <IconButton
                        style={{margin: 0}}
                        icon="arrow-down-bold"
                        onPress={() => viewModel.reorderStepForADay(step.id, 0)}
                        disabled={isLast}
                    />
                </View>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width40]}>
                <DeliveryTypeIcon type={step.type}/>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width110]}>
                {step.deliveryCode}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width180]}>
                <Text numberOfLines={3}>{step.address.streetInfo}</Text>
                <Text numberOfLines={3}>{`${step.address.postcode} ${step.address.city}`}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {step.packing}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <TextInput
                    value={viewModel.comment}
                    onChangeText={viewModel.setComment}
                    mode="outlined"
                />
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={3}>{step.courierComment}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width90]}>
                {step.deliveryDate}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {step.deliveryTime}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {step.deliveryLimitDate}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                <Text style={{textAlign: 'center', width: '100%'}}>{step.courierCode}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {
                    !canChangeDate ? (
                        <Text>{step.estimatedTime}</Text>
                    ) : (
                        <TimePickerInput
                            hours={parseInt(viewModel.splitTime[0]) ?? 0}
                            minutes={parseInt(viewModel.splitTime[1]) ?? 0}
                            onOpen={() => setIsTimePickerOpen(true)}
                            onClose={() => setIsTimePickerOpen(false)}
                            onConfirm={({hours, minutes}: {
                                hours: number;
                                minutes: number;
                            }): void => viewModel.updateStepTimeForADay(step.id, hours, minutes)}
                        />
                    )
                }
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width40]}>
                {
                    step.completed ? (
                        <Icon source="check-circle-outline" size={28} color={theme.colors.onBackground}/>
                    ) : (
                        <Button
                            mode="contained"
                            onPress={() => setIsCompletionDatePickerOpen(true)}
                            compact
                        >
                            Valider
                        </Button>
                    )
                }
            </DataTable.Cell>
            <DatePickerModal
                locale="fr"
                mode="single"
                visible={isCompletionDatePickerOpen}
                onDismiss={() => setIsCompletionDatePickerOpen(false)}
                date={completionDate}
                onConfirm={onConfirmDatePicker}
            />
            <TimePickerModal
                locale="fr"
                visible={isCompletionTimePickerOpen}
                onDismiss={() => setIsCompletionTimePickerOpen(false)}
                onConfirm={onConfirmTimePicker}
                hours={new Date().getHours()}
                minutes={new Date().getMinutes()}
            />
        </DataTable.Row>
    </SafeAreaProvider>
    );
}