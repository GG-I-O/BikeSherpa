import { DataTable, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import datatableStyle from "@/style/datatableStyle";
import StepDataTableRowAssign from "./StepDataTableRowAssign";
import { ScrollView } from "react-native";

type Props = {
    steps: Array<Step>,
    isStepSelected?: (step: Step) => boolean,
    onRowPress?: (step: Step) => void,
    canChangeDate?: boolean,
    showHeader?: boolean
}

export default function StepDataTableAssign({ steps, isStepSelected, onRowPress, canChangeDate = false, showHeader = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    return (
        <ScrollView>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                {showHeader ? (
                    <DataTable.Header>
                        <DataTable.Title style={[style.column, style.typeColumn]}>Trier</DataTable.Title>
                        <DataTable.Title style={[style.column, style.typeColumn]}>Type</DataTable.Title>
                        <DataTable.Title style={[style.column, style.courseColumn]}>Code</DataTable.Title>
                        <DataTable.Title style={[style.column, style.dateColumn]}>Date</DataTable.Title>
                        <DataTable.Title style={[style.column, style.timeColumn]}>Heure contrat</DataTable.Title>
                        <DataTable.Title style={[style.column, style.descriptionColumn]}>Description</DataTable.Title>
                        <DataTable.Title style={[style.column, style.descriptionColumn]}>Commentaire</DataTable.Title>
                        <DataTable.Title style={[style.column, style.adresseColumn]}>Adresse</DataTable.Title>
                        <DataTable.Title style={[style.column, style.timeIncrementColumn]}>Heure</DataTable.Title>
                    </DataTable.Header>
                ) : (
                    <></>
                )}

                {steps.map((step) => (
                    <StepDataTableRowAssign
                        key={`${step.id}`}
                        step={step}
                        isSelected={isStepSelected ? isStepSelected(step) : false}
                        onPress={onRowPress}
                        canChangeDate={canChangeDate}
                    />
                ))}
            </DataTable>
        </ScrollView>
    );
}