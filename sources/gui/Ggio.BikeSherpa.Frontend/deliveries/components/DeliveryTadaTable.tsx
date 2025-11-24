import { DataTable, useTheme } from "react-native-paper";
import { Delivery } from "../models/Delivery";
import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import DeliveryDataTableRow from "./DeliveryDataTableRow";
import { Step } from "@/steps/models/Step";

type Props = {
    deliveries: Array<Delivery>,
    isDeliverySelected?: (delivery: Delivery) => boolean,
    isStepSelected?: (step: Step) => boolean,
    onDeliveryPress?: (delivery: Delivery) => void,
    onStepPress?: (step: Step, delivery: Delivery) => void,
    onDetails?: (delivery: Delivery) => void,
    onEdit?: (delivery: Delivery) => void,
    onCopy?: (delivery: Delivery) => void,
    onDelete?: (delivery: Delivery) => void
}

export default function DeliveryDataTable({ deliveries, isDeliverySelected, isStepSelected, onDeliveryPress, onStepPress, onDetails, onEdit, onCopy, onDelete }: Props) {
    const theme = useTheme();

    const style = datatableStyle;

    return (
        <ScrollView>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                <DataTable.Header>
                    <DataTable.Title style={[style.column]}>Code</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Client</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Nb étapes</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Date</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Horaire de début</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Actions</DataTable.Title>
                </DataTable.Header>

                {deliveries.map((delivery) => (
                    <DeliveryDataTableRow
                        delivery={delivery}
                        isSelected={isDeliverySelected ? isDeliverySelected(delivery) : false}
                        isStepSelected={isStepSelected}
                        onPress={onDeliveryPress}
                        onStepPress={onStepPress}
                        onDetails={onDetails ? onDetails : undefined}
                        onEdit={onEdit ? onEdit : undefined}
                        onCopy={onCopy ? onCopy : undefined}
                        onDelete={onDelete ? onDelete : undefined}
                    />
                ))}
            </DataTable>
        </ScrollView>
    );
}