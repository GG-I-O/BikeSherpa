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
    
    public async getReport(customerId: string, startDate: string, endDate: string): Promise<Report> {
        const data = await this.apiClient.GetReportEndpoint({
            queries: {
                customerId: customerId,
                startDate: startDate,
                endDate: endDate
            }
        });

        return {
            customerName: data.customerName,
            startDate: DateToolbox.getFormattedDateFromISO(new Date(data.startDate).toISOString()),
            endDate: DateToolbox.getFormattedDateFromISO(new Date(data.endDate).toISOString()),
            totalPrice: data.totalPrice,
            deliveries: data.deliveries.map(delivery => ({
                deliveryCode: delivery.deliveryCode,
                deliveryDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.deliveryDate).toISOString()),
                deliveryTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.deliveryDate).toISOString()),
                deliveryPrice: delivery.deliveryPrice,
                details: delivery.details
            }))
        };
    }
}