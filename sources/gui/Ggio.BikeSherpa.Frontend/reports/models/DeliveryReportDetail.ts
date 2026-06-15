import {Address} from "@/models/Address";

export type DeliveryReportDetail = {
    description: string;
    address: Address | null;
    price: number;
    quantity: number;
}