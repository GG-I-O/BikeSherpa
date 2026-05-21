import AppStyle from "@/constants/AppStyle";
import { View } from "react-native";
import { Text, useTheme } from "react-native-paper";
import {Report} from "@/reports/models/Report";
import ReportDataTable from "@/reports/components/ReportDataTable";

type Props = {
    report: Report | null
}

export default function ReportDetail({report} : Props) {
    const theme = useTheme();

    if (report === null)
        return (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Aucune course pour ce client et cette période</Text>
            </View>
        );

    return (
        <View style={{ backgroundColor: theme.colors.background, padding: 8, height: '100%' }}>
            <View style={{ flexDirection: 'row', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8 }}>
                <Text style={AppStyle.textStyle.h3}>
                    {report.customerName}
                </Text>
                <Text style={AppStyle.textStyle.h3}>
                    {`${report.startDate} --- ${report.endDate}`} 
                </Text>
            </View>
            <Text style={[AppStyle.textStyle.h3,{textAlign: 'right'}]}>{report.totalPrice} €</Text>
            <ReportDataTable report={report} />
        </View>
    );
}