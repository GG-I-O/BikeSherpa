import useDeliveryViewModel from "@/deliveries/viewModel/DeliveryViewModel";
import Report from "../models/Report";

class ReportViewModel {
    private static instance: ReportViewModel;
    private reports: Array<Report>;

    public static getInstance(): ReportViewModel {
        if (!ReportViewModel.instance)
            ReportViewModel.instance = new ReportViewModel();
        return ReportViewModel.instance;
    }

    private constructor() {
        const viewModel = useDeliveryViewModel();
        this.reports = [];
    }

    public getReportList(): Array<Report> {
        return this.reports;
    }

    public getReport(reportId: string): Report | undefined {
        return this.reports.find((report) => report.id == reportId);
    }

}

export default function useReportViewModel() {
    return ReportViewModel.getInstance();
}