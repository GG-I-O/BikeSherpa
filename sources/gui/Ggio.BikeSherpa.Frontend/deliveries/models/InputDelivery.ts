import {Step} from "@/steps/models/Step";

export default class InputDelivery {
    public code: string;
    public status: number;
    public customerId: string;
    public pricingStrategy: number;
    public urgency: string;
    public totalPrice: number | null;
    public discount: number | null;
    public discountReason: string | null;
    public extraCost: number | null;
    public extraCostReason: string | null;
    public steps: Step[];
    public details: string[];
    public insulatedBox: boolean;
    public contractDate: string;
    public startDate: string;
    public limitDate?: string | null;
    public needEstimate?: boolean;
    public customerReference: string | null;

    public constructor(
        code: string,
        status: number,
        customerId: string,
        pricingStrategy: number,
        urgency: string,
        totalPrice: number, 
        discount: number,
        discountReason: string,
        extraCost: number,
        extraCostReason: string,
        steps: Step[],
        details: string[],
        insulatedBox: boolean,
        contractDate: string,
        startDate: string,
        limitDate: string,
        needEstimate: boolean,
        customerReference: string,
    ) {
        this.code = code;
        this.status = status;
        this.customerId = customerId;
        this.pricingStrategy = pricingStrategy;
        this.urgency = urgency;
        this.totalPrice = totalPrice;
        this.discount = discount;
        this.discountReason = discountReason;
        this.extraCost = extraCost;
        this.extraCostReason = extraCostReason;
        this.steps = steps;
        this.details = details;
        this.insulatedBox = insulatedBox;
        this.contractDate = contractDate;
        this.startDate = startDate;
        this.limitDate = limitDate;
        this.needEstimate = needEstimate;
        this.customerReference = customerReference;
    }
}