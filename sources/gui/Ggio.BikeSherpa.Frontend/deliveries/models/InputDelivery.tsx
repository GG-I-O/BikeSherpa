import DeliveryDetail from "./DeliveryDetail";
import { DeliveryPacking } from "./DeliveryPacking";

export class InputDelivery {
    public code: string;
    public customerId: string;
    public totalPrice: number;
    public reportId: string;
    public stepIds: string[];
    public details: DeliveryDetail[];
    public packing: DeliveryPacking;

    public constructor(code: string, customerId: string, totalPrice: number, reportId: string, stepIds: string[] = [], details: DeliveryDetail[] = [], packing: DeliveryPacking) {
        this.code = code;
        this.customerId = customerId;
        this.totalPrice = totalPrice;
        this.reportId = reportId;
        this.stepIds = stepIds;
        this.details = details;
        this.packing = packing;
    }
}