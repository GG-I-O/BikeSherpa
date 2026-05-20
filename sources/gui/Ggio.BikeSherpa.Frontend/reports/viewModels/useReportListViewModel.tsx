import {useEffect, useState} from "react";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import ReportListViewModel from "@/reports/viewModels/ReportListViewModel";
import {observe} from "@legendapp/state";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {ICustomerService} from "@/spi/CustomerSPI";
import defaultDropdownOption from "@/deliveries/data/defaultDropdownOption";
import DateToolbox from "@/services/DateToolbox";
import {IReportServices} from "@/reports/spi/IReportServices";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import {Report} from "@/reports/models/Report";

export default function useReportListViewModel() {
    const reportServices = IOCContainer.get<IReportServices>(ReportServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new ReportListViewModel(reportServices, customerServices);
    
    const customerStore$ = customerServices.getCustomerList$();
    
    const [reports, setReports] = useState<Report[]>([]);
    const [startDateFilter, setStartDateFilter] = useState<Date>(DateToolbox.getFirstDayOfTheMonth(new Date()));
    const [endDateFilter, setEndDateFilter] = useState<Date>(DateToolbox.getLastDayOfTheMonth(new Date()));
    const [customerFilter, setCustomerFilter] = useState<string | undefined>();

    const [customersOptions, setCustomersOptions] = useState<{ label: string, value: string }[]>([]);
    
    useEffect(() => {
        return observe(() => {
          viewModel.getReports(startDateFilter, endDateFilter, customerFilter !== defaultDropdownOption[0].value ? customerFilter : undefined)
              .then(reports => setReports(reports));
          
            let customerList: { label: string, value: string }[] = [];
            customerList.push(...defaultDropdownOption);
            Object.values(customerStore$.peek()).forEach(customer =>
                customerList.push({label: customer.code + " - " + customer.name, value: customer.code})
            );
            setCustomersOptions(customerList);
        });
    }, [customerStore$, setReports, setCustomersOptions, startDateFilter, endDateFilter, customerFilter])
    
    return {
        reports,
        setReports,
        startDateFilter,
        setStartDateFilter,
        endDateFilter,
        setEndDateFilter,
        customerFilter,
        setCustomerFilter,
        customersOptions
    };
}