import AppStyle from "@/constants/AppStyle";
import { useLocalSearchParams } from "expo-router";
import { useEffect, useState } from "react";
import { View } from "react-native";
import { Text, useTheme } from "react-native-paper";
import { useNavigation } from "@react-navigation/native";
import ReportDeliveryList from "../components/ReportDeliveryList";
import useReportViewModel from "../viewModels/ReportViewModel";
import { Delivery } from "@/deliveries/models/Delivery";
import Report from "../models/Report";

export default function ReportDetailView() {
    const theme = useTheme();
    const { reportId } = useLocalSearchParams<{ reportId: string }>();
    const viewModel = useReportViewModel();
    const [report, setReport] = useState<Report | undefined>();
    const nav = useNavigation();

    useEffect(() => {
        setReport(viewModel.getReport(reportId));
    }, [viewModel, reportId]);

    useEffect(() => {
        nav.setOptions({ headerTitle: report ? report.reportDate : 'Détails rapport' });
    }, [report, nav])

    if (!report)
        return (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Rapport inexistant</Text>
            </View>
        );

    return (
        <View style={{ backgroundColor: theme.colors.background, padding: 8, height: '100%' }}>
            <View style={{ flexDirection: 'row', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8 }}>
                <Text style={AppStyle.textStyle.h3}>
                    {report.customer}
                </Text>
                <Text style={AppStyle.textStyle.h3}>
                    {report.reportDate.toLocaleDateString()}
                </Text>
            </View>
            <ReportDeliveryList deliveries={report.deliveries} />
        </View>
    );
}