import TimePickerInput from "@/components/general/TimePickerInput";
import { useState } from "react";
import { Button, DataTable, IconButton, Text, TextInput, useTheme } from "react-native-paper";
import { DatePickerInput } from "react-native-paper-dates";
import { Step } from "@/steps/models/Step";
import datatableStyle from "@/style/datatableStyle";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import { StepType } from "@/steps/models/StepType";

type Props = {
    step: Step,
    deleteRow: (step: Step) => void
}

export default function StepRowInput({ step, deleteRow }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    // Input useState
    // TODO: change it for react-hook-form
    const [type, setType] = useState<StepType>(step.type);
    const [deliveryDate, setDeliveryDate] = useState<Date>(step.contractDate);
    const [contractTime, setContractTime] = useState<{ hours: number, minutes: number }>({ hours: step.contractDate.getHours(), minutes: step.contractDate.getMinutes() })
    const [description, setDescription] = useState<string>(step.description ?? '');
    const [comment, setComment] = useState<string>(step.comment ?? '');
    const [address, setAddress] = useState<string>(step.address.streetInfo ? `${step.address.streetInfo} ${step.address.postcode} ${step.address.city}` : '');

    return (
        <DataTable.Row style={{ padding: 0 }}>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.buttonColumn]}>
                <IconButton icon="trash-can-outline" onPress={() => deleteRow(step)} />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.buttonColumn]}>
                <Button
                    mode='outlined'
                    onPress={() => setType(type == StepType.PickUp ? StepType.Drop : StepType.PickUp)}
                >
                    {/* Wrap the Icon into a text to be able to reduce margin */}
                    <Text style={{ marginInline: 0 }}>
                        <DeliveryTypeIcon type={type} />
                    </Text>
                </Button>
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <DatePickerInput
                    locale={"fr"}
                    inputMode={"start"}
                    onChange={(date: Date | undefined): void => {
                        let auxDate: Date;
                        if (date)
                            auxDate = date;
                        else
                            auxDate = new Date();
                        auxDate.setHours(contractTime.hours, contractTime.minutes, 0, 0);
                        setDeliveryDate(auxDate);
                    }}
                    value={deliveryDate}
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.buttonColumn]}>
                <TimePickerInput
                    hours={contractTime.hours}
                    minutes={contractTime.minutes}
                    onConfirm={({ hours, minutes }: { hours: number; minutes: number; }): void => {
                        setContractTime({ hours, minutes });
                    }}
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <TextInput
                    style={{ width: '100%' }}
                    label="Description"
                    value={description}
                    onChangeText={text => setDescription(text)}
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <TextInput
                    style={{ width: '100%' }}
                    label="Commentaire"
                    value={comment}
                    onChangeText={text => setComment(text)}
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <TextInput
                    style={{ width: '100%' }}
                    label="Adresse"
                    value={address}
                    onChangeText={text => setAddress(text)}
                />
            </DataTable.Cell>
        </DataTable.Row>
    );
}