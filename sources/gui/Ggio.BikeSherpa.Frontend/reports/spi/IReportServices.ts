import {Report} from "@/reports/models/Report";

export interface IReportServices {
    getReports(customerId: string, startDate: string, endDate: string): Promise<Report[]>;
}