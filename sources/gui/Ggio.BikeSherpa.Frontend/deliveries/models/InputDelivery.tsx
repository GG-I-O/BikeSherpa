import DeliveryDetail from "./DeliveryDetail";
import { DeliveryStatus } from "./DeliveryStatus";
import { PackageSize } from "./PackageSize";

export class InputDelivery {
    public deliveryStatus: DeliveryStatus;
    public code: string;
    public customerId: string;
    public totalPrice: number;
    public reportId: string;
    public stepIds: string[];
    public details: DeliveryDetail[];
    public packing: PackageSize;

    public constructor(deliveryStatus: DeliveryStatus, code: string, customerId: string, totalPrice: number, reportId: string, stepIds: string[] = [], details: DeliveryDetail[] = [], packing: PackageSize) {
        this.deliveryStatus = deliveryStatus;
        this.code = code;
        this.customerId = customerId;
        this.totalPrice = totalPrice;
        this.reportId = reportId;
        this.stepIds = stepIds;
        this.details = details;
        this.packing = packing;
    }
}