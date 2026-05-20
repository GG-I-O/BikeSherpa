import {inject} from "inversify";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import {IReportServices} from "@/reports/spi/IReportServices";
import {Report} from "@/reports/models/Report";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

export default class ReportListViewModel {
    private readonly reportServices: IReportServices;
    private readonly customerServices: ICustomerService;

    constructor(
        @inject(ReportServiceIdentifier.Services) reportServices: IReportServices,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.reportServices = reportServices;
        this.customerServices = customerServices;
    }

    public getReports = async (startDateFilter: Date, endDateFilter: Date, customerFilter?: string): Promise<Report[]> => {
        if (!this.reportServices || !this.customerServices || !customerFilter) return [];
        
        const customerId = this.customerServices.getCustomerIdByCode(customerFilter);
        if (!customerId) return [];
        const customer = this.customerServices.getCustomer$(customerId).get();

        const reports = await this.reportServices.getReports(customerId, startDateFilter.toISOString(), endDateFilter.toISOString());
        if (reports.length > 0)
            reports[0].customer = customer.name;
        return reports;
    }
}