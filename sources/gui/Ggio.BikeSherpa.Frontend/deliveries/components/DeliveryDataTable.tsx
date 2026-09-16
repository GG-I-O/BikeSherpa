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
                    <DataTable.Title style={[style.column, style.width50]}>Status</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width180]}>Code</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Infos de la course</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Nb étapes</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width100]}>Prix</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width40]}>Com. Liv.</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width130]}>Date</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width80]}>Heure début</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width80]}>Heure limite</DataTable.Title>
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