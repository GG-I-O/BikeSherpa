import { ScrollView } from "react-native";
import { DataTable, useTheme } from "react-native-paper";
import ReportDataTableRow from "./ReportDataTableRow";
import datatableStyle from "@/style/datatableStyle";
import {Report} from "@/reports/models/Report";

type Props = {
    reports: Report[]
}

export default function ReportDataTable(props: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    return <ScrollView>
        <DataTable style={{ backgroundColor: theme.colors.background }}>
            <DataTable.Header>
                <DataTable.Title style={[style.column]}>Description</DataTable.Title>
                <DataTable.Title style={[style.column, style.width80]}>Prix</DataTable.Title>
                <DataTable.Title style={[style.column, style.width80]}>Quantité</DataTable.Title>
            </DataTable.Header>

            {props.reports.map((report, index) => (
                <ReportDataTableRow
                    report={report}
                    key={index}
                />
            ))}
        </DataTable>
    </ScrollView>
} 