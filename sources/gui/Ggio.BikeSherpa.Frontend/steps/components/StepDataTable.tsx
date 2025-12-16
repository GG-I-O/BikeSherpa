import { DataTable, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import datatableStyle from "@/style/datatableStyle";
import StepDataTableRow from "./StepDataTableRow";

type Props = {
    steps: Step[],
    isStepSelected?: (step: Step) => boolean,
    onRowPress?: (step: Step) => void,
    canChangeDate?: boolean,
    showHeader?: boolean
}

export default function StepDataTable({ steps, isStepSelected, onRowPress, canChangeDate = false, showHeader = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    return (
        <DataTable style={{ backgroundColor: theme.colors.background }}>
            {showHeader ? (
                <DataTable.Header>
                    <DataTable.Title style={[style.column, style.width40]}>Type</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width90]}>Date</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Heure contrat</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth100]}>Description</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth100]}>Commentaire</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth150]}>Adresse</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Livreur</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Heure</DataTable.Title>
                </DataTable.Header>
            ) : (
                <></>
            )}

            {steps.map((step) => (
                <StepDataTableRow
                    key={`${step.id}`}
                    step={step}
                    isSelected={isStepSelected ? isStepSelected(step) : false}
                    onPress={onRowPress}
                    canChangeDate={canChangeDate}
                />
            ))}
        </DataTable>
    );
}