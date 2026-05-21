import {IReportServices} from "@/reports/spi/IReportServices";
import {injectable} from "inversify";
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";
import {Report} from "@/reports/models/Report";
import DateToolbox from "@/services/DateToolbox";

@injectable()
export default class ReportServices implements IReportServices {
    private apiClient;
    
    public constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }
    
    public async getReports(customerId: string, startDate: string, endDate: string): Promise<Report[]> {
        const reports = await this.apiClient.GetAllReportsEndpoint({
            queries: {
                customerId: customerId,
                startDate: startDate,
                endDate: endDate
            }
        });
        
        return reports.map(report => {
            return {
                deliveryCode: report.deliveryCode,
                deliveryDate: DateToolbox.getFormattedDateFromISO(new Date(report.deliveryDate).toISOString()),
                deliveryTime: DateToolbox.getFormattedTimeFromISO(new Date(report.deliveryDate).toISOString()),
                deliveryPrice: report.deliveryPrice,
                details: report.details
            };
        })
    }
}