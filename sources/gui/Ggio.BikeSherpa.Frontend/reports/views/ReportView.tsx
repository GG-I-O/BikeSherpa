import {Linking, ScrollView, View} from "react-native";
import {Button, Text, useTheme, SegmentedButtons} from "react-native-paper";
import {DatePickerModal} from "react-native-paper-dates";
import DateToolbox from "@/services/DateToolbox";
import React, {useState} from "react";
import {Dropdown} from "react-native-paper-dropdown";
import ReportDetail from "@/reports/components/ReportDetail";
import useReportViewModel from "@/reports/viewModels/useReportViewModel";


export default function ReportView() {
    const viewModel = useReportViewModel();
    const theme = useTheme();

    const [openDatePicker, setOpenDatePicker] = useState<boolean>(false);

    return (
        <>
            <ScrollView
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
                <SegmentedButtons
                    value={viewModel.reportType}
                    onValueChange={viewModel.setReportType}
                    buttons={viewModel.reportTypeValues.map(b => ({
                        ...b,
                        style: {width: 100}
                    }))}
                />
                <Button
                    mode="outlined"
                    onPress={() => setOpenDatePicker(true)}
                >
                    <Text>{DateToolbox.getFormattedDateFromISO(viewModel.startDateFilter.toISOString())} --- {DateToolbox.getFormattedDateFromISO(viewModel.endDateFilter.toISOString())}</Text>
                </Button>
                {viewModel.reportType === 'Client' && (
                    <Dropdown
                        key={`courier-filter-${viewModel.customerFilter || 'empty'}`}
                        label="Client"
                        options={viewModel.customersOptions.slice(1)}
                        value={viewModel.customerFilter}
                        onSelect={(value) => viewModel.setCustomerFilter(value)}
                        mode="outlined"

                    />
                )}
                
                <DatePickerModal
                    locale="fr"
                    mode="range"
                    visible={openDatePicker}
                    startDate={viewModel.startDateFilter}
                    endDate={viewModel.endDateFilter}
                    onDismiss={() => setOpenDatePicker(false)}
                    onConfirm={({startDate, endDate}) => {
                        if (!startDate || !endDate) return;
                        viewModel.setStartDateFilter(startDate);
                        viewModel.setEndDateFilter(endDate);
                        setOpenDatePicker(false);
                    }}
                />
            </ScrollView>
            {viewModel.reportType === 'Coursier' && viewModel.courierReportPath && (
                <View style={{alignItems: 'center', justifyContent: 'center', marginTop: 16}}>
                    <Text style={{
                        fontWeight: 'bold',
                        marginBottom: 8
                    }}>{viewModel.courierReportPath?.Name}</Text>
                    <Text style={{color: 'blue'}}
                          onPress={() => Linking.openURL(viewModel.courierReportPath?.Path!)}>
                        Télécharger le rapport des coursiers
                    </Text>

                </View>
            )}

            {viewModel.reportType === 'Client' && viewModel.report && (
                <View style={{ flexDirection: 'row', gap: 8, paddingLeft: 8, backgroundColor: theme.colors.background}}>
                    <Button
                        mode="outlined"
                        onPress={() => viewModel.exportCustomerReport()}
                    >
                        Générer le rapport client
                    </Button>
                    {viewModel.customerReportExportUrl && (
                        <Button
                            mode="outlined"
                            onPress={() => Linking.openURL(viewModel.customerReportExportUrl!)}
                            icon="download"
                        >
                            Voir le rapport client
                        </Button>
                    )}
                </View>
            )}

            {!viewModel.customerFilter && !viewModel.courierReportPath ? (
                <View style={{alignItems: 'center', justifyContent: 'center'}}>
                    <Text style={{color: "orange"}}>Sélectionne une période et un client pour voir le rapport</Text>
                </View>
            ) : (
                <ReportDetail
                    report={viewModel.report}
                />
            )}
        </>
    );
}