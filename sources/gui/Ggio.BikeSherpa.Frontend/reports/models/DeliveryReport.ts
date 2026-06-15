import {DeliveryReportDetail} from "@/reports/models/DeliveryReportDetail";

export type DeliveryReport = {
    deliveryCode: string;
    deliveryDate: string;
    deliveryTime: string;
    deliveryPrice: number;
    deliveryPriceWithVat: number;
    details: DeliveryReportDetail[];
}