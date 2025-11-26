import { Step } from "@/steps/models/Step";
import Customer from "@/customers/models/Customer";
import PriceDetail from "../../models/PriceDetail";
import { DeliveryPacking } from "./DeliveryPacking";
import ValidationOptions from "@/models/ValidationOptions";

export default class InputDelivery {
    public code: string;
    public customer: Customer;
    public totalPrice: number;
    public reportId: string;
    public packing: DeliveryPacking;
    public validationOptions: ValidationOptions;
    public steps: Step[];
    public priceDetails: PriceDetail[];

    public constructor(
        code: string,
        customer: Customer,
        totalPrice: number,
        reportId: string,
        packing: DeliveryPacking,
        validationOptions: ValidationOptions,
        steps: Step[] = [],
        priceDetails: PriceDetail[] = []
    ) {
        this.code = code;
        this.customer = customer;
        this.totalPrice = totalPrice;
        this.reportId = reportId;
        this.packing = packing;
        this.validationOptions = validationOptions;
        this.steps = steps;
        this.priceDetails = priceDetails;
    }
}