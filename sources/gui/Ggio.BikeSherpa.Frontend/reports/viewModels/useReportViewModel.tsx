import {useEffect, useState} from "react";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {observe} from "@legendapp/state";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {ICustomerService} from "@/spi/CustomerSPI";
import defaultDropdownOption from "@/deliveries/data/defaultDropdownOption";
import DateToolbox from "@/services/DateToolbox";
import {IReportServices} from "@/reports/spi/IReportServices";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import {Report} from "@/reports/models/Report";
import ReportViewModel from "@/reports/viewModels/ReportViewModel";

export default function useReportViewModel() {
    const reportServices = IOCContainer.get<IReportServices>(ReportServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new ReportViewModel(reportServices);

    const customerStore$ = customerServices.getCustomerList$();

    const [report, setReport] = useState<Report | null>(null);
    const [startDateFilter, setStartDateFilter] = useState<Date>(DateToolbox.getFirstDayOfTheMonth(new Date()));
    const [endDateFilter, setEndDateFilter] = useState<Date>(DateToolbox.getLastDayOfTheMonth(new Date()));
    const [customerFilter, setCustomerFilter] = useState<string | undefined>();

    const [customersOptions, setCustomersOptions] = useState<{ label: string, value: string }[]>([]);

    useEffect(() => {
        return observe(() => {
            viewModel.getReport(startDateFilter, endDateFilter, customerFilter !== defaultDropdownOption[0].value ? customerFilter : undefined)
                .then(report => setReport(report));

            let customerList: { label: string, value: string }[] = [];
            customerList.push(...defaultDropdownOption);

            Object.values(customerStore$.peek()).forEach(customer => {
                    customer ? customerList.push({label: customer.code + " - " + customer.name, value: customer.id}) : {};
                }
            );
            setCustomersOptions(customerList);
        });
    }, [customerStore$, setReport, setCustomersOptions, startDateFilter, endDateFilter, customerFilter])

    return {
        report,
        setReport,
        startDateFilter,
        setStartDateFilter,
        endDateFilter,
        setEndDateFilter,
        customerFilter,
        setCustomerFilter,
        customersOptions
    };
}