import { useEffect, useState } from "react";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { observe } from "@legendapp/state";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ICustomerService } from "@/spi/CustomerSPI";
import defaultDropdownOption from "@/deliveries/data/defaultDropdownOption";
import DateToolbox from "@/services/DateToolbox";
import { IReportServices } from "@/reports/spi/IReportServices";
import { ReportServiceIdentifier } from "@/reports/bootstrapper/ReportServiceIdentifier";
import { Report } from "@/reports/models/Report";
import ReportViewModel from "@/reports/viewModels/ReportViewModel";
import { ICourierService } from "@/spi/CourierSPI";

export interface CourierReportResult{
    Name: string;
    Path: string;
}

export default function useReportViewModel() {
    const reportServices = IOCContainer.get<IReportServices>(ReportServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const viewModel = new ReportViewModel(reportServices);

    const customerStore$ = customerServices.getCustomerList$();
    const courierStore$ = courierServices.getCourierList$();

    const [report, setReport] = useState<Report | null>(null);
    const [startDateFilter, setStartDateFilter] = useState<Date>(DateToolbox.getFirstDayOfTheMonth(new Date()));
    const [endDateFilter, setEndDateFilter] = useState<Date>(DateToolbox.getLastDayOfTheMonth(new Date()));

    const [customerFilter, setCustomerFilter] = useState<string | undefined>();
    const [customersOptions, setCustomersOptions] = useState<{ label: string, value: string }[]>([]);

    const [courierReportPath, setCourierReportPath] = useState<CourierReportResult | null>(null);
    const [courierFilter, setCourierFilter] = useState<string | undefined>();
    const [couriersOptions, setCourierOptions] = useState<{ label: string, value: string }[]>([]);

    const reportTypeValues = [{
        label: 'Client', value: 'Client'
    },
    {
        label: 'Coursier', value: 'Coursier'
    }] as const;

    const [reportType, setReportType] = useState<'Client' | 'Coursier'>('Client');

    useEffect(() => {
        return observe(() => {
            viewModel.getCustomerReport(startDateFilter, endDateFilter, customerFilter !== defaultDropdownOption[0].value ? customerFilter : undefined)
                .then(report => setReport(report));

            let customerList: { label: string, value: string }[] = [];
            customerList.push(...defaultDropdownOption);

            Object.values(customerStore$.peek()).forEach(customer => {
                customer ? customerList.push({ label: customer.code + " - " + customer.name, value: customer.id }) : {};
            }
            );
            setCustomersOptions(customerList);

        });
    }, [customerStore$, setReport, setCustomersOptions, startDateFilter, endDateFilter, customerFilter]);

    useEffect(() => {
        // For courier management
        setCourierOptions([...defaultDropdownOption, ...Object.values(courierStore$.peek()).map(courier => ({
            label: courier.code + " - " + courier.firstName,
            value: courier.id
        }))]);

        if (courierFilter) {
            viewModel.getCourierReport(startDateFilter, endDateFilter, courierFilter).then(path => {
                if (path) {
                    // Handle the path to the downloaded report file
                    console.log("Courier report downloaded at:", path);
                    const currentCourier = courierStore$.peek()[courierFilter];
                    const reportName = "Rapport coursier " + currentCourier?.firstName + " " + currentCourier?.lastName + " du " + DateToolbox.getFormattedDateFromISO(startDateFilter.toISOString()) + " au " + DateToolbox.getFormattedDateFromISO(endDateFilter.toISOString());
                    setCourierReportPath({ Name : reportName , Path : path });
                }
            });
        }

    }, [courierStore$, setCourierOptions, setCourierReportPath, startDateFilter, endDateFilter, courierFilter]);

    return {
        report,
        setReport,
        startDateFilter,
        setStartDateFilter,
        endDateFilter,
        setEndDateFilter,
        customerFilter,
        setCustomerFilter,
        customersOptions,
        reportType,
        setReportType,
        reportTypeValues,
        courierFilter,
        setCourierFilter,
        couriersOptions,
        courierReportPath
    };
}