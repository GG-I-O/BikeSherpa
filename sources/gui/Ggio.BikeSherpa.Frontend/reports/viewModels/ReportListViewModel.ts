import {inject} from "inversify";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import {IReportServices} from "@/reports/spi/IReportServices";
import {Report} from "@/reports/models/Report";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

export default class ReportListViewModel {
    private readonly reportServices: IReportServices;

    constructor(
        @inject(ReportServiceIdentifier.Services) reportServices: IReportServices
    ) {
        this.reportServices = reportServices;
    }

    public getReport = async (startDateFilter: Date, endDateFilter: Date, customerFilter?: string): Promise<Report | null> => {
        if (!this.reportServices || !customerFilter) return null;

        return await this.reportServices.getReport(customerFilter, startDateFilter.toISOString(), endDateFilter.toISOString());
    }
}