import { IReportServices } from "@/reports/spi/IReportServices";
import { injectable } from "inversify";
import { createApiClient } from "@/infra/openAPI/client";
import axios from "axios";
import { Report } from "@/reports/models/Report";
import DateToolbox from "@/services/DateToolbox";

@injectable()
export default class ReportServices implements IReportServices {
    private readonly apiClient;

    public constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
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

   public async getCustomeReportExportUrl(customerId: string, startDate : string, endDate : string)
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

        return {
            customerName: data.customerName,
            startDate: DateToolbox.getFormattedDateFromISO(new Date(data.startDate).toISOString()),
            endDate: DateToolbox.getFormattedDateFromISO(new Date(data.endDate).toISOString()),
            totalPrice: data.totalPrice,
            totalPriceWithVat: data.totalPriceWithVat,
            deliveries: data.deliveries.map(delivery => ({
                deliveryCode: delivery.deliveryCode,
                deliveryDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.deliveryDate).toISOString()),
                deliveryTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.deliveryDate).toISOString()),
                deliveryPrice: delivery.deliveryPrice,
                deliveryPriceWithVat: delivery.deliveryPriceWithVat,
                details: delivery.details.map(detail => ({
                    ...detail,
                    address: {
                        ...detail.address,
                        name: detail.address?.name ?? "",
                        streetInfo: detail.address?.streetInfo ?? "",
                        postcode: detail.address?.postcode ?? "",
                        city: detail.address?.city ?? "",
                        complement: detail.address?.complement ?? "",
                        phone: detail.address?.phone ?? "",
                        coordinates: detail.address?.coordinates ?? { longitude: 0, latitude: 0 },
                        fullAddress: detail.address ? `${detail.address.name} - ${detail.address.streetInfo} ${detail.address.postcode} ${detail.address.city}` : ""
                    }
                }))
            }))
        };
    }
}