import AppStyle from "@/constants/AppStyle";
import { View } from "react-native";
import { Text, useTheme } from "react-native-paper";
import DateToolbox from "@/services/DateToolbox";
import {Report} from "@/reports/models/Report";
import ReportDataTable from "@/reports/components/ReportDataTable";

type Props = {
    customerName: string;
    startDate: string;
    endDate: string;
    totalPrice: number;
    reports: Report[];
}

export default function ReportDetail(props : Props) {
    const theme = useTheme();

    if (props.reports.length < 1)
        return (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Aucune course pour ce client et cette période</Text>
            </View>
        );

    return (
        <View style={{ backgroundColor: theme.colors.background, padding: 8, height: '100%' }}>
            <View style={{ flexDirection: 'row', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8 }}>
                <Text style={AppStyle.textStyle.h3}>
                    {props.customerName}
                </Text>
                <Text style={AppStyle.textStyle.h3}>
                    {DateToolbox.getFormattedDateFromISO(props.startDate)} --- {DateToolbox.getFormattedDateFromISO(props.endDate)} 
                </Text>
            </View>
            <Text style={[AppStyle.textStyle.h3,{textAlign: 'right'}]}>{props.totalPrice}€</Text>
            <ReportDataTable reports={props.reports} />
        </View>
    );
}