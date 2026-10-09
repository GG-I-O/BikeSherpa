import { IReportServices } from "@/reports/spi/IReportServices";
import {inject, injectable} from "inversify";
import { createApiClient } from "@/infra/openAPI/client";
import axios from "axios";
import { Report } from "@/reports/models/Report";
import IReportMapper from "@/reports/spi/IReportMapper";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";

@injectable()
export default class ReportServices implements IReportServices {
    private readonly apiClient;
    private readonly reportMapper: IReportMapper;

    public constructor(
        @inject(ReportServiceIdentifier.Mapper) reportMapper: IReportMapper
    ) {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
        this.reportMapper = reportMapper;
    }
    public async getCourierReportUrl(courierId: string, startDate: string, endDate: string): Promise<string> {
       
        return await this.apiClient.GetCourierReport({
            params: {
                courierId: courierId
            },
            queries: {
                startDate: startDate,
                endDate: endDate
            }
        });
    }

   public async getCustomerReportExportUrl(customerId: string, startDate : string, endDate : string)
    {
        return await this.apiClient.ExportCustomerReport({
            params: {
                customerId: customerId
            },
            queries: {
                from: startDate,
                to: endDate
            }
        })
    }

    public async getCustomerReport(customerId: string, startDate: string, endDate: string): Promise<Report> {
        const data = await this.apiClient.GetCustomerReport({
            params: {
                customerId: customerId
            },
            queries: {
                startDate: startDate,
                endDate: endDate
            }
        });

        return this.reportMapper.openAPIReportToReport(data);
    }
    
    public async getDeliveryReport(deliveryId: string): Promise<Report> {
        const data = await this.apiClient.GetDeliveryReport({
            params: {
                deliveryId: deliveryId
            }
        });
        
        return this.reportMapper.openAPIReportToReport(data);
    }
}