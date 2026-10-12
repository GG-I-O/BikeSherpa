import {injectable} from "inversify";
import IReportMapper from "@/reports/spi/IReportMapper";
import DateToolbox from "@/services/DateToolbox";
import { Report } from "@/reports/models/Report";
import {DeliveryReport} from "@/reports/models/DeliveryReport";

@injectable()
export default class ReportMapper implements IReportMapper {
    public openAPIReportToReport(openAPIReport: any): Report {
        return {
            customerName: openAPIReport.customerName,
            startDate: DateToolbox.getFormattedDateFromISO(new Date(openAPIReport.startDate).toISOString()),
            endDate: DateToolbox.getFormattedDateFromISO(new Date(openAPIReport.endDate).toISOString()),
            totalPrice: openAPIReport.totalPrice,
            totalPriceWithVat: openAPIReport.totalPriceWithVat,
            deliveries: openAPIReport.deliveries.map((delivery: DeliveryReport) => ({
                deliveryLabel: delivery.deliveryLabel,
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