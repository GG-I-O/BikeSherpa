import {DataTable, useTheme} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import StepDataTableRowAssign from "./StepDataTableRowAssign";
import {ScrollView} from "react-native";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {Step} from "@/steps/models/Step";

type Props = {
    steps: StepToDisplay[],
    isStepSelected?: (step: StepToDisplay) => boolean,
    onRowPress?: (step: StepToDisplay) => void,
    canChangeDate?: boolean,
    showHeader?: boolean
}

export default function StepDataTableAssign(
    {
        steps,
        isStepSelected,
        onRowPress,
        canChangeDate = false,
        showHeader = false
    }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    // stepDate to decide first/last step for a date
    let stepDate: string = '';

    return (
        <ScrollView>
            <DataTable style={{backgroundColor: theme.colors.background}}>
                {showHeader ? (
                    <DataTable.Header>
                        <DataTable.Title style={[style.column, style.width40]}>Ordre</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width40]}>Type</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width110]}>Code</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width180]}>Adresse</DataTable.Title>
                        <DataTable.Title style={[style.column, style.minWidth150]}>Commentaire</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width90]}>Date</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width60]}>Heure début</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width60]}>Heure limite</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width60]}>Livreur</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width60]}>Heure</DataTable.Title>
                        <DataTable.Title style={[style.column, style.width40]}>Finis</DataTable.Title>
                    </DataTable.Header>
                ) : (
                    <></>
                )}

                {steps.map((step, index) => {
                        const prev = steps[index - 1];
                        const next = steps[index + 1];

                        const isSameGroup = (a: StepToDisplay, b: StepToDisplay) =>
                            a &&
                            b &&
                            a.courierCode === b.courierCode &&
                            a.estimatedDate === b.estimatedDate;

                        const isFirst = !isSameGroup(step, prev);
                        const isLast = !isSameGroup(step, next);

                        return (
                            <StepDataTableRowAssign
                                key={`${step.id}`}
                                step={step}
                                isSelected={isStepSelected ? isStepSelected(step) : false}
                                onPress={onRowPress}
                                canChangeDate={canChangeDate}
                                isFirst={isFirst}
                                isLast={isLast}
                            />
                        )
                    }
                )}
            </DataTable>
        </ScrollView>
    );
}