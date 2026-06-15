import {ScrollView} from "react-native";
import {DataTable, useTheme} from "react-native-paper";
import ReportDataTableRow from "./ReportDataTableRow";
import datatableStyle from "@/style/datatableStyle";
import {Report} from "@/reports/models/Report";

type Props = {
    report: Report
}

export default function ReportDataTable({report}: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    return <ScrollView>
        <DataTable style={{backgroundColor: theme.colors.background, marginBottom: 100}}>
            <DataTable.Header>
                <DataTable.Title style={[style.column]}>Description</DataTable.Title>
                <DataTable.Title style={[style.column, style.width80]}>Prix</DataTable.Title>
            </DataTable.Header>

            {report.deliveries.map((deliveryReport, index) => (
                <ReportDataTableRow
                    report={deliveryReport}
                    key={index}
                />
            ))}
        </DataTable>
    </ScrollView>
} 