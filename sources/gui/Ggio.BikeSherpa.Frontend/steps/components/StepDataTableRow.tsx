import { DataTable, Text, useTheme } from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import { useState } from "react";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import TimePickerInput from "@/components/general/TimePickerInput";
import {Icon} from "react-native-paper/src";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

type Props = {
    step: StepToDisplay,
    isSelected?: boolean,
    onPress?: (step: StepToDisplay) => void,
    canChangeDate?: boolean
}

export default function StepDataTableRow({ step, isSelected = false, onPress, canChangeDate = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    
    const splitTime = step.estimatedTime.split(':');
    const [contractTime, setContractTime] = useState<{ hours: number, minutes: number }>(
        {
            hours: parseInt(splitTime[0]) ?? 0,
            minutes: parseInt(splitTime[1]) ?? 0
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
                <DeliveryTypeIcon type={step.type} />
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={3}>{step.address.streetInfo}</Text>
                <Text numberOfLines={3}>{`${step.address.postcode} ${step.address.city}`}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={2}>{step.comment}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth100]}>{step.comment}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>{step.courierCode}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>
                {
                    !canChangeDate ? (
                        <Text>{step.estimatedDate}</Text>
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