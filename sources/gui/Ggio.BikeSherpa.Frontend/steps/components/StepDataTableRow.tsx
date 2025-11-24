import { DataTable, Text, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import datatableStyle from "@/style/datatableStyle";
import { useState } from "react";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import TimePickerInput from "@/components/general/TimePickerInput";

type Props = {
    step: Step,
    isSelected?: boolean,
    onPress?: (step: Step) => void,
    canChangeDate?: boolean
}

export default function StepDataTableRow({ step, isSelected = false, onPress, canChangeDate = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    const [contractTime, setContractTime] = useState<{ hours: number, minutes: number }>(
        {
            hours: step.estimatedDate?.getHours() ?? 0,
            minutes: step.estimatedDate?.getMinutes() ?? 0
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
            <DataTable.Cell style={[style.column, style.typeColumn]}>
                <DeliveryTypeIcon type={step.type} />
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.dateColumn]}>{step.getContractDate()}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.timeColumn]}>{step.getContractTime()}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.descriptionColumn]}>
                <Text numberOfLines={2}>{step.description}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.descriptionColumn]}>{step.comment}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.adresseColumn]}>
                <Text numberOfLines={3}>{step.address.streetInfo}</Text>
                <Text numberOfLines={3}>{`${step.address.postcode} ${step.address.city}`}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.timeColumn]}>{step.courier}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.timeColumn]}>
                {
                    !canChangeDate ? (
                        <Text>{step.getEstimatedTime()}</Text>
                    ) : (
                        <TimePickerInput
                            hours={contractTime.hours}
                            minutes={contractTime.minutes}
                            onOpen={() => setIsTimePickerOpen(true)}
                            onClose={() => setIsTimePickerOpen(false)}
                            onConfirm={({ hours, minutes }: { hours: number; minutes: number; }): void => {
                                setContractTime({ hours, minutes });
                                step.estimatedDate = new Date(step.contractDate);
                                step.estimatedDate?.setHours(hours, minutes, 0, 0);
                            }}
                        />
                    )
                }
            </DataTable.Cell>
        </DataTable.Row >
    );
}