import { Card, Divider, IconButton, Text, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import { View } from "react-native";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import { Dropdown } from "react-native-paper-dropdown";
import TimePickerInput from "@/components/general/TimePickerInput";
import { useEffect, useState } from "react";

type Props = {
    step: Step,
    onAssign: (step: Step, courier: string) => void,
    onTimePicker: (step: Step, time: Date) => void,
    onDelete?: (step: Step) => void
}

export default function StepCardAssign({ step, onAssign, onTimePicker, onDelete }: Props) {
    const theme = useTheme();

    const [courierDropDown, setCourierDropdown] = useState<string>(step.courier ?? 'NONE');
    const [timePicker, setTimePicker] = useState<{ hours: number, minutes: number }>({ hours: step.estimatedDate?.getHours() ?? 0, minutes: step.estimatedDate?.getMinutes() ?? 0 });

    useEffect(() => {
        onAssign(step, courierDropDown);
    }, [courierDropDown]);

    useEffect(() => {
        const date = new Date(step.contractDate);
        date.setHours(timePicker.hours, timePicker.minutes, 0, 0);
        onTimePicker(step, date);
    }, [timePicker]);

    return (
        <Card
            style={{ backgroundColor: theme.colors.background }}
        >
            <Card.Content>
                <View style={{ flexDirection: 'row', justifyContent: 'space-evenly', alignItems: 'center' }}>
                    <DeliveryTypeIcon type={step.type} />
                    <Text>{step.getContractDate()}</Text>
                    <Text>{step.getContractTime()}</Text>
                    <IconButton style={{ margin: 0 }} icon="trash-can-outline" onPress={() => onDelete ? onDelete(step) : {}} />
                </View>
                <Divider />

                <View style={{ gap: 4, marginBlock: 8 }}>
                    <Text>{step.address.name}</Text>
                    <Text>{step.address.streetInfo}</Text>
                    <Text>{`${step.address.postcode} ${step.address.city}`}</Text>
                    <Divider />
                    <Text numberOfLines={2}>{step.description}</Text>
                    <Text numberOfLines={2}>{step.details?.size}</Text>
                </View>

                <Divider />
                <View style={{ gap: 4, marginBlock: 8, alignItems: 'center' }}>
                    <Dropdown
                        label="Assignées à"
                        options={[]}
                        value={courierDropDown}
                        onSelect={(value?: string) => setCourierDropdown(value ?? 'NONE')}
                    />
                    <TimePickerInput
                        hours={timePicker.hours}
                        minutes={timePicker.minutes}
                        onConfirm={({ hours, minutes }: { hours: number, minutes: number }) => {
                            setTimePicker({ hours, minutes });
                        }}
                    />
                </View>
            </Card.Content>
        </Card>
    );
}