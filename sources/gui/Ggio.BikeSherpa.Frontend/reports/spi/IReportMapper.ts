import { Report } from "@/reports/models/Report";

export default interface IReportMapper {
    openAPIReportToReport(openAPIReport: any): Report;
}