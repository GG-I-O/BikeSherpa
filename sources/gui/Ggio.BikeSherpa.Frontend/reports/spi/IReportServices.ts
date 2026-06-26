import {Report} from "@/reports/models/Report";

export interface IReportServices {
    getCustomerReport(customerId: string, startDate: string, endDate: string): Promise<Report>;
    getCourierReportUrl(courierId: string, startDate: string, endDate: string): Promise<string>;
}