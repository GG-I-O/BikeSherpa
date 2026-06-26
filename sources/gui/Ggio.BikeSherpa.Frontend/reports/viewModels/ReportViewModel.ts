import {inject} from "inversify";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import {IReportServices} from "@/reports/spi/IReportServices";
import {Report} from "@/reports/models/Report";
export default class ReportViewModel {
    private readonly reportServices: IReportServices;

    constructor(
        @inject(ReportServiceIdentifier.Services) reportServices: IReportServices
    ) {
        this.reportServices = reportServices;
    }

    public getCustomerReport = async (startDateFilter: Date, endDateFilter: Date, customerFilter?: string): Promise<Report | null> => {
        if (!this.reportServices || !customerFilter) return null;

        return await this.reportServices.getCustomerReport(customerFilter, startDateFilter.toISOString(), endDateFilter.toISOString());
    }

    public getCourierReport = async (startDateFilter: Date, endDateFilter: Date, courierFilter?: string): Promise<string | null> => {
        if (!this.reportServices || !courierFilter) return null;

        const path = await this.reportServices.getCourierReportUrl(courierFilter, startDateFilter.toISOString(), endDateFilter.toISOString());

        return path;
    }

}