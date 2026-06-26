import { Linking, ScrollView, View } from "react-native";
import { Button, Text, useTheme, SegmentedButtons } from "react-native-paper";
import { DatePickerModal } from "react-native-paper-dates";
import DateToolbox from "@/services/DateToolbox";
import React, { useState } from "react";
import { Dropdown } from "react-native-paper-dropdown";
import ReportDetail from "@/reports/components/ReportDetail";
import useReportViewModel from "@/reports/viewModels/useReportViewModel";



export default function ReportView() {
    const viewModel = useReportViewModel();
    const theme = useTheme();

    const [openDatePicker, setOpenDatePicker] = useState<boolean>(false);

    return <>

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
                    style: { width: 100 }
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

            {viewModel.reportType === 'Coursier' && (
                <Dropdown
                    key={`courier-filter-${viewModel.courierFilter || 'empty'}`}
                    label="Coursier"
                    options={viewModel.couriersOptions.slice(1)}
                    value={viewModel.courierFilter}
                    onSelect={(value) => viewModel.setCourierFilter(value)}
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
                onConfirm={({ startDate, endDate }) => {
                    if (!startDate || !endDate) return;
                    viewModel.setStartDateFilter(startDate);
                    viewModel.setEndDateFilter(endDate);
                    setOpenDatePicker(false);
                }}
            />
        </ScrollView>
        {!viewModel.customerFilter ? (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Selectionne une période et un client pour voir le rapport</Text>
            </View>
        ) : (
            <ReportDetail
                report={viewModel.report}
            />
        )}

        {viewModel.reportType === 'Coursier' && viewModel.courierReportPath && (
            <View style={{ alignItems: 'center', justifyContent: 'center', marginTop: 16 }}>
                <Text style={{ fontWeight: 'bold', marginBottom: 8 , color: "orange" }}>{viewModel.courierReportPath?.Name}</Text>
                <Text style={{ color: 'blue' }}
                    onPress={() => Linking.openURL( viewModel.courierReportPath?.Path! )}>
                     Télécharger le rapport du coursier
                </Text>
               
            </View>
        )}
    </>
}