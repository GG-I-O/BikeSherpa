import { DataTable, IconButton, Text, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import datatableStyle from "@/style/datatableStyle";
import { useState } from "react";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import TimePickerInput from "@/components/general/TimePickerInput";
import { View } from "react-native";

type Props = {
    step: Step,
    isSelected?: boolean,
    onPress?: (step: Step) => void,
    canChangeDate?: boolean
}

export default function StepDataTableRowAssign({ step, isSelected = false, onPress, canChangeDate = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    const [heureContrat, setHeureContrat] = useState<{ hours: number, minutes: number }>(
        {
            hours: step.estimatedDate?.getHours() ?? 0,
            minutes: step.estimatedDate?.getMinutes() ?? 0
        }
    )
    const [isTimePickerOpen, setIsTimePickerOpen] = useState(false); // Disable onRowPress if we're picking time

    const changeTime = (hours: number, minutes: number) => {
        setHeureContrat({ hours, minutes });
        step.estimatedDate = new Date(step.contractDate);
        step.estimatedDate?.setHours(hours, minutes, 0, 0);
    }

    return (
        <DataTable.Row
            onPress={() => {
                if (isTimePickerOpen) return;
                if (onPress) onPress(step);
            }}
            style={{ backgroundColor: isSelected ? theme.colors.primary : theme.colors.background }}
        >
            <DataTable.Cell style={[style.column, style.width40]}>
                <View>
                    <IconButton style={{ margin: 0, borderRadius: 5, height: '35%' }} mode='outlined' icon='arrow-up-thick' />
                    <IconButton style={{ margin: 0, borderRadius: 5, height: '35%' }} mode='outlined' icon='arrow-down-thick' />
                </View>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width40]}>
                <DeliveryTypeIcon type={step.type} />
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width100]}>{step.deliveryCode}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width90]}>{step.getContractDate()}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width60]}>{step.getContractTime()}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth100]}>
                <Text numberOfLines={2}>{step.description}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth100]}>{step.comment}</DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth150]}>
                <Text numberOfLines={3}>{step.address.streetInfo}</Text>
                <Text numberOfLines={3}>{`${step.address.postcode} ${step.address.city}`}</Text>
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.timeIncrementColumn]}>
                {
                    !canChangeDate ? (
                        <Text>{step.getEstimatedTime()}</Text>
                    ) : (
                        <View style={{ flexDirection: 'row', gap: 4 }}>
                            <IconButton
                                style={{ margin: 0, borderRadius: 5, width: 30 }}
                                mode='outlined'
                                icon='minus'
                                onPress={() => {
                                    let hours = heureContrat.hours;
                                    let minutes = heureContrat.minutes - 5;
                                    if (minutes < 0) {
                                        hours--;
                                        minutes += 60;
                                    }
                                    changeTime(hours, minutes);
                                }}
                            />

                            <TimePickerInput
                                hours={heureContrat.hours}
                                minutes={heureContrat.minutes}
                                onOpen={() => setIsTimePickerOpen(true)}
                                onClose={() => setIsTimePickerOpen(false)}
                                onConfirm={({ hours, minutes }: { hours: number, minutes: number }): void => {
                                    changeTime(hours, minutes);
                                }}
                            />
                            <IconButton
                                style={{ margin: 0, borderRadius: 5, width: 30 }}
                                mode='outlined'
                                icon='plus'
                                onPress={() => {
                                    let hours = heureContrat.hours;
                                    let minutes = heureContrat.minutes + 5;
                                    if (minutes >= 60) {
                                        hours++;
                                        minutes -= 60;
                                    }
                                    changeTime(hours, minutes);
                                }}
                            />
                        </View>
                    )
                }
            </DataTable.Cell>
        </DataTable.Row >
    );
}