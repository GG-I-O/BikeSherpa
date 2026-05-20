import {ReportDetail} from "@/reports/models/ReportDetail";

export type Report = {
    customer?: string | undefined;
    deliveryCode: string;
    deliveryDate: string;
    deliveryTime: string;
    deliveryPrice: number;
    details: ReportDetail[];
}