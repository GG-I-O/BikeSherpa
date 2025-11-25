import { navigate } from "expo-router/build/global-state/routing";
import { useEffect, useState } from "react";
import { ScrollView, View } from "react-native";
import { Searchbar, Text, useTheme } from "react-native-paper";
import { DatePickerInput } from "react-native-paper-dates";
import Report from "../models/Report";
import ReportDataTable from "../components/ReportDataTable";
import useReportViewModel from "../viewModels/ReportViewModel";

export default function ReportListView() {
    const viewModel = useReportViewModel();
    const theme = useTheme();
    const [reports, setReports] = useState<Report[]>([]);

    useEffect(() => {
        setReports(viewModel.getReportList());
    }, [viewModel]);

    if (reports.length < 1) {
        return (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Aucun rapport à afficher</Text>
            </View>
        );
    }

    return <> <ScrollView
        horizontal
        style={{
            flexDirection: 'row', flexGrow: 0, flexShrink: 0,
            backgroundColor: theme.colors.background,
            padding: 8
        }}
        contentContainerStyle={{
            alignItems: "center", gap: 8
        }}
    >
        <Searchbar value={""} />
        <DatePickerInput
            inputMode={"end"}
            onChange={(date: Date | undefined) => { }}
            value={undefined} locale={"fr"} mode="outlined" />
    </ScrollView>

        <ReportDataTable
            reports={reports}
            onPress={(report: Report) => navigate({
                pathname: '/(tabs)/(reports)/[reportId]',
                params: { reportId: report.id }
            })} />
    </>
}