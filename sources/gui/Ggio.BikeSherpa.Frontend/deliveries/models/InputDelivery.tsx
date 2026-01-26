import { Step } from "@/steps/models/Step";
import DeliveryDetail from "./DeliveryDetail";
import { DeliveryPacking } from "./DeliveryPacking";
import Customer from "@/customers/models/Customer";

export class InputDelivery {
    public code: string;
    public customer: Customer;
    public totalPrice: number;
    public reportId: string;
    public steps: Step[];
    public details: DeliveryDetail[];
    public packing: DeliveryPacking;

    public constructor(code: string, customer: Customer, totalPrice: number, reportId: string, steps: Step[] = [], details: DeliveryDetail[] = [], packing: DeliveryPacking) {
        this.code = code;
        this.customer = customer;
        this.totalPrice = totalPrice;
        this.reportId = reportId;
        this.steps = steps;
        this.details = details;
        this.packing = packing;
    }
}