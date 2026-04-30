import { DataTable, useTheme } from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import DeliveryDataTableRow from "./DeliveryDataTableRow";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

type Props = {
    deliveries: DeliveryToDisplay[],
    isDeliverySelected?: (delivery: DeliveryToDisplay) => boolean,
    isStepSelected?: (step: StepToDisplay) => boolean,
    onDeliveryPress?: (delivery: DeliveryToDisplay) => void,
    onStepPress?: (step: StepToDisplay, delivery: DeliveryToDisplay) => void,
    onDetails?: (delivery: DeliveryToDisplay) => void,
    onEdit?: (delivery: DeliveryToDisplay) => void,
    onCopy?: (delivery: DeliveryToDisplay) => void,
    onDelete?: (delivery: DeliveryToDisplay) => void
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
                    <DataTable.Title style={[style.column]}>Prix</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Date</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Heure début</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Heure limite</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width180]}>Actions</DataTable.Title>
                </DataTable.Header>

                {deliveries.map((delivery) => (
                    <DeliveryDataTableRow
                        key={delivery.id}
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