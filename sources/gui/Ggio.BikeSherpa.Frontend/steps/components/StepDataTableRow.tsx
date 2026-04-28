import {DataTable, IconButton, Text, useTheme} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import React, {useState} from "react";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import TimePickerInput from "@/components/general/TimePickerInput";
import {Icon} from "react-native-paper/src";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import useStepDataTableRowViewModel from "@/steps/viewModel/useStepDataTableRowViewModel";
import {View} from "react-native";

type Props = {
    step: StepToDisplay,
    isSelected?: boolean,
    onPress?: (step: StepToDisplay) => void,
    canChangeDate?: boolean,
    listLength?: number
}

export default function StepDataTableRow({step, isSelected = false, onPress, canChangeDate = false, listLength = 0}: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    const splitTime = step.estimatedTime.split(':');
    const [isTimePickerOpen, setIsTimePickerOpen] = useState(false); // Disable onRowPress if we're picking time

    const viewModel = useStepDataTableRowViewModel();

    return (
        <DataTable.Row
            onPress={() => {
                if (isTimePickerOpen) return;
                if (onPress) onPress(step);
            }}
            style={{backgroundColor: isSelected ? theme.colors.primary : theme.colors.background}}
        >
            {canChangeDate && (
                <DataTable.Cell style={[style.column, style.width40]}>
                    <View style={{flexDirection: "column", gap: 0}}>
                        <IconButton
                            style={{margin: 0}}
                            icon="arrow-up-bold"
                            onPress={() => viewModel.reorderStep(step.id, step.order - 1)}
                            disabled={step.order <= 1}
                        />
                        <IconButton
                            style={{margin: 0}}
                            icon="arrow-down-bold"
                            onPress={() => viewModel.reorderStep(step.id, step.order + 1)}
                            disabled={step.order >= listLength}
                        />
                    </View>
                </DataTable.Cell>
            )}

            <DataTable.Cell style={[style.column, style.width40]}>
                <DeliveryTypeIcon type={step.type}/>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={3}>{step.address.streetInfo}</Text>
                <Text numberOfLines={3}>{`${step.address.postcode} ${step.address.city}`}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={2}>{step.comment}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>{step.courierCode}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {
                    !canChangeDate ? (
                        <Text>{step.estimatedTime}</Text>
                    ) : (
                        <TimePickerInput
                            hours={parseInt(splitTime[0]) ?? 0}
                            minutes={parseInt(splitTime[1]) ?? 0}
                            onOpen={() => setIsTimePickerOpen(true)}
                            onClose={() => setIsTimePickerOpen(false)}
                            onConfirm={({hours, minutes}: {
                                hours: number;
                                minutes: number;
                            }): void => viewModel.updateStepTime(step.id, hours, minutes)}
                        />
                    )
                }
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width40]}>
                {
                    step.completed && (
                        <Icon source="check-circle-outline" size={28} color={theme.colors.onBackground}/>
                    )
                }
            </DataTable.Cell>
        </DataTable.Row>
    );
}