import {DeliveryReport} from "@/reports/models/DeliveryReport";

export type Report = {
    customerName: string;
    startDate: string;
    endDate: string;
    totalPrice: number;
    deliveries: DeliveryReport[];
}