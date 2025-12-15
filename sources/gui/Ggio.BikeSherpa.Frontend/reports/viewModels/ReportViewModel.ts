import Report from "../models/Report";

class ReportViewModel {
    private static instance: ReportViewModel;
    private readonly reports: Report[];

    public static getInstance(): ReportViewModel {
        if (!ReportViewModel.instance)
            ReportViewModel.instance = new ReportViewModel();
        return ReportViewModel.instance;
    }

    private constructor() {
        this.reports = [];
    }

    public getReportList(): Report[] {
        return this.reports;
    }

    public getReport(reportId: string): Report | undefined {
        return this.reports.find((report) => report.id === reportId);
    }

}

export default function useReportViewModel() {
    return ReportViewModel.getInstance();
}