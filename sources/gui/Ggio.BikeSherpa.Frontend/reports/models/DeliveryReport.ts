import {DeliveryReportDetail} from "@/reports/models/DeliveryReportDetail";

export type DeliveryReport = {
    deliveryLabel: string;
    deliveryPrice: number;
    deliveryPriceWithVat: number;
    details: DeliveryReportDetail[];
}