import {ScrollView, View} from "react-native";
import {Button, Text, useTheme} from "react-native-paper";
import {DatePickerModal} from "react-native-paper-dates";
import useReportListViewModel from "@/reports/viewModels/useReportListViewModel";
import DateToolbox from "@/services/DateToolbox";
import React, {useState} from "react";
import {Dropdown} from "react-native-paper-dropdown";
import ReportDetail from "@/reports/components/ReportDetail";

export default function ReportView() {
    const viewModel = useReportListViewModel();
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
            <Button
                mode="outlined"
                onPress={() => setOpenDatePicker(true)}
            >
                <Text>{DateToolbox.getFormattedDateFromISO(viewModel.startDateFilter.toISOString())} --- {DateToolbox.getFormattedDateFromISO(viewModel.endDateFilter.toISOString())}</Text>
            </Button>
            <Dropdown
                key={`courier-filter-${viewModel.customerFilter || 'empty'}`}
                label="Client"
                options={viewModel.customersOptions.slice(1)}
                value={viewModel.customerFilter}
                onSelect={(value) => viewModel.setCustomerFilter(value)}
                mode="outlined"
            />
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
        {!viewModel.customerFilter ? (
            <View style={{alignItems: 'center', justifyContent: 'center'}}>
                <Text>Selectionne une période et un client pour voir le rapport</Text>
            </View>
        ) : (
            <ReportDetail
                reports={viewModel.reports}
                startDate={viewModel.startDateFilter.toISOString()}
                endDate={viewModel.endDateFilter.toISOString()}
                customerName={viewModel.reports.length > 0 ? (viewModel.reports[0].customer ?? '') : ''}
                totalPrice={viewModel.reports.reduce((accumulator, report) => {
                    return accumulator + report.deliveryPrice
                },0)}
            />
        )}
    </>
}