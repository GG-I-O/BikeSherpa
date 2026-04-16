import { DataTable, Text, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import datatableStyle from "@/style/datatableStyle";
import { useState } from "react";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import TimePickerInput from "@/components/general/TimePickerInput";
import DateToolbox from "@/services/DateToolbox";
import {Icon} from "react-native-paper/src";
import {StepType} from "@/steps/models/StepType";

type Props = {
    step: Step,
    isSelected?: boolean,
    onPress?: (step: Step) => void,
    canChangeDate?: boolean
}

export default function StepDataTableRow({ step, isSelected = false, onPress, canChangeDate = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    
    const stepEstimatedDate = new Date(step.estimatedDeliveryDate);
    const [contractTime, setContractTime] = useState<{ hours: number, minutes: number }>(
        {
            hours: stepEstimatedDate.getHours() ?? 0,
            minutes: stepEstimatedDate.getMinutes() ?? 0
        }
    )
    const [isTimePickerOpen, setIsTimePickerOpen] = useState(false); // Disable onRowPress if we're picking time

    return (
        <DataTable.Row
            onPress={() => {
                if (isTimePickerOpen) return;
                if (onPress) onPress(step);
            }}
            style={{ backgroundColor: isSelected ? theme.colors.primary : theme.colors.background }}
        >
            <DataTable.Cell style={[style.column, style.width40]}>
                <DeliveryTypeIcon type={step.stepType} />
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={3}>{step.stepAddress.streetInfo}</Text>
                <Text numberOfLines={3}>{`${step.stepAddress.postcode} ${step.stepAddress.city}`}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={2}>{step.comment}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth100]}>{step.comment}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>{step.courierId}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {
                    !canChangeDate ? (
                        <Text>{step.estimatedDeliveryDate}</Text>
                    ) : (
                        <TimePickerInput
                            hours={contractTime.hours}
                            minutes={contractTime.minutes}
                            onOpen={() => setIsTimePickerOpen(true)}
                            onClose={() => setIsTimePickerOpen(false)}
                            onConfirm={({ hours, minutes }: { hours: number; minutes: number; }): void => {
                                setContractTime({ hours, minutes });
                                /*const stepDate = new Date(step.estimatedDeliveryDate);
                                stepDate.setHours(hours, minutes, 0, 0);
                                step.estimatedDeliveryDate = stepDate.toISOString();*/
                            }}
                        />
                    )
                }
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width40]}>
                {
                    step.completed ? (
                        <Icon source="check-circle-outline" size={28} color={theme.colors.onBackground} />
                    ) : (
                        <></>
                    )
                }
            </DataTable.Cell>
        </DataTable.Row >
    );
}