import { DataTable, Text, useTheme } from "react-native-paper";
import { Step } from "@/steps/models/Step";
import datatableStyle from "@/style/datatableStyle";
import StepRowInput from "./StepRowInput";

type Props = {
    steps: Array<Step>,
    deleteRow: (step: Step) => void
}

export default function StepInputDataTable({ steps, deleteRow }: Props) {
    const theme = useTheme();

    return (
        <DataTable style={{ backgroundColor: theme.colors.background }}>
            <DataTable.Header style={{ padding: 0 }}>
                <DataTable.Title style={[datatableStyle.column, datatableStyle.buttonColumn]}>
                    <Text>Actions</Text>
                </DataTable.Title>
                <DataTable.Title style={[datatableStyle.column, datatableStyle.buttonColumn]}>
                    <Text>Type</Text>
                </DataTable.Title>
                <DataTable.Title style={[datatableStyle.column]}>
                    <Text>Date</Text>
                </DataTable.Title>
                <DataTable.Title style={[datatableStyle.column, datatableStyle.buttonColumn]}>
                    <Text>Heure</Text>
                </DataTable.Title>
                <DataTable.Title style={[datatableStyle.column]}>
                    <Text>Description</Text>
                </DataTable.Title>
                <DataTable.Title style={[datatableStyle.column]}>
                    <Text>Commentaire</Text>
                </DataTable.Title>
                <DataTable.Title style={[datatableStyle.column]}>
                    <Text>Adresse</Text>
                </DataTable.Title>
            </DataTable.Header>

            {steps.map((step) => (
                <StepRowInput
                    key={step.id}
                    step={step}
                    deleteRow={deleteRow}
                />
            ))}
        </DataTable>
    );
}