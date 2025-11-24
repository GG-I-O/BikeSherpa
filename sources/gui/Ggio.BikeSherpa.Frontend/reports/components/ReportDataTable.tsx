import { ScrollView } from "react-native";
import { DataTable, useTheme } from "react-native-paper";
import ReportDataTableRow from "./ReportDataTableRow";
import datatableStyle from "@/style/datatableStyle";
import Report from "../models/Report";

type Props = {
    reports: Report[],
    onPress?: (report: Report) => void,
}

export default function ReportDataTable(props: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    return <ScrollView>
        <DataTable style={{ backgroundColor: theme.colors.background }}>
            <DataTable.Header>
                <DataTable.Title style={[style.column]}>Client</DataTable.Title>
                <DataTable.Title style={[style.column]}>Numéro de rapport</DataTable.Title>
                <DataTable.Title style={[style.column]}>Date</DataTable.Title>
                <DataTable.Title style={[style.column]}>Courses</DataTable.Title>
            </DataTable.Header>

            {props.reports.map((report, index) => (
                <ReportDataTableRow
                    report={report}
                    onPress={props.onPress}
                    key={index}
                />
            ))}
        </DataTable>
    </ScrollView>
} 