import { DataTable, useTheme } from "react-native-paper";
import Report from "../models/Report";
import datatableStyle from "@/style/datatableStyle";

type Props = {
    report: Report,
    onPress?: (report: Report) => void,
}

export default function ReportDataTableRow({ report, onPress }: Props) {
    const theme = useTheme();

    const style = datatableStyle;

    return (
        <>
            <DataTable.Row
                key={report.id}
                testID="ReportDataTableRow"
                onPress={() => {
                    if (onPress) return onPress(report);
                }}
                style={{ backgroundColor: theme.colors.background }}
            >
                <DataTable.Cell style={[style.column]}>
                    {report.customer}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column]}>
                    {report.reportNumber}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column]}>
                    {report.reportDate.toLocaleDateString()}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column,]}>
                    {report.deliveries.length}
                </DataTable.Cell>
            </DataTable.Row>
        </>
    );
}