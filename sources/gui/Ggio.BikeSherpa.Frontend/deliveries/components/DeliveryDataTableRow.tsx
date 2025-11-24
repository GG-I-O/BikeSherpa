import { DataTable, IconButton, useTheme } from "react-native-paper";
import { Delivery } from "../models/Delivery";
import datatableStyle from "@/style/datatableStyle";
import { View } from "react-native";
import { useState } from "react";
import StepDataTable from "@/steps/components/StepDataTable";
import { Step } from "@/steps/models/Step";

type Props = {
    delivery: Delivery,
    isSelected?: boolean,
    isStepSelected?: (step: Step) => boolean,
    onPress?: (delivery: Delivery) => void,
    onStepPress?: (step: Step, delivery: Delivery) => void,
    onDetails?: (delivery: Delivery) => void,
    onEdit?: (delivery: Delivery) => void,
    onCopy?: (delivery: Delivery) => void,
    onDelete?: (delivery: Delivery) => void
}

export default function DeliveryDataTableRow({ delivery, isSelected = false, isStepSelected, onPress, onStepPress, onDetails, onEdit, onCopy, onDelete }: Props) {
    const theme = useTheme();

    const style = datatableStyle;

    const [showSteps, setShowSteps] = useState<boolean>(false);

    return (
        <>
            <DataTable.Row
                onPress={() => {
                    if (onPress) return onPress(delivery);
                    setShowSteps(!showSteps);
                }}
                style={{ backgroundColor: isSelected ? theme.colors.primary : theme.colors.background }}
            >
                <DataTable.Cell style={[style.column]}>
                    {delivery.code}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column]}>
                    {delivery.customer.name}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column]}>
                    {delivery.steps?.length}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column]}>
                    {delivery.steps ? delivery.steps[0].getContractDate() : ''}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column,]}>
                    {delivery.steps ? delivery.steps[0].getContractTime() : ''}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column,]}>
                    <IconButton style={{ margin: 0 }} icon="magnify" onPress={() => onDetails ? onDetails(delivery) : {}} />
                    <IconButton style={{ margin: 0 }} icon="pencil" onPress={() => onEdit ? onEdit(delivery) : {}} />
                    <IconButton style={{ margin: 0 }} icon="content-copy" onPress={() => onCopy ? onCopy(delivery) : {}} />
                    <IconButton style={{ margin: 0 }} icon="trash-can-outline" onPress={() => onDelete ? onDelete(delivery) : {}} />
                </DataTable.Cell>
            </DataTable.Row>
            {showSteps ? (
                <View style={{ marginInline: 16, borderWidth: 1, borderColor: theme.colors.surfaceVariant }}>
                    <StepDataTable
                        steps={delivery.steps ?? []}
                        isStepSelected={isStepSelected}
                        onRowPress={onStepPress ? (step: Step) => onStepPress(step, delivery) : undefined}
                        canChangeDate={true}
                    />
                </View>
            ) : (
                <></>
            )}
        </>
    );
}