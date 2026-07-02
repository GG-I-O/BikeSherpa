import { DataTable, useTheme } from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import StepDataTableRow from "./StepDataTableRow";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IColorServiceSpi} from "@/spi/ColorServiceSpi";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

type Props = {
    steps: StepToDisplay[],
    isStepSelected?: (step: StepToDisplay) => boolean,
    onRowPress?: (step: StepToDisplay) => void,
    canChangeDate?: boolean,
    showHeader?: boolean
}

export default function StepDataTable({ steps, isStepSelected, onRowPress, canChangeDate = false, showHeader = false }: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    const colorService = IOCContainer.get<IColorServiceSpi>(ServicesIdentifiers.ColorService);

    const getHeaderBackgroundColor = () => {
        if (steps.length === 0) return theme.colors.background;
        const firstStepCode = steps[0].deliveryCode;
        if (!firstStepCode) return theme.colors.background;

        const color = colorService.stringToColor(firstStepCode);
        return color + '20';
    };

    return (
        <DataTable style={{ backgroundColor: theme.colors.background }}>
            {showHeader && (
                <DataTable.Header style={{ backgroundColor: getHeaderBackgroundColor() }}>
                    { canChangeDate && (
                        <DataTable.Title style={[style.column, style.width40]}>Ordre</DataTable.Title>
                    )}
                    <DataTable.Title style={[style.column, style.width40]}>Type</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth150]}>Adresse</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Colisage</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth150]}>Comm</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth150]}>Comm Livreur</DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth150]}>Pièces jointes</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Livreur</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width60]}>Heure</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width40]}>Finis</DataTable.Title>
                </DataTable.Header>
            )}

            {steps.map((step) => (
                <StepDataTableRow
                    key={`${step.id}`}
                    step={step}
                    isSelected={isStepSelected ? isStepSelected(step) : false}
                    onPress={onRowPress}
                    canChangeDate={canChangeDate}
                    listLength={steps.length}
                />
            ))}
        </DataTable>
    );
}