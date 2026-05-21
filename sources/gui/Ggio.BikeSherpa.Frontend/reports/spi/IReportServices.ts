import {Report} from "@/reports/models/Report";

export interface IReportServices {
    getReport(customerId: string, startDate: string, endDate: string): Promise<Report>;
}