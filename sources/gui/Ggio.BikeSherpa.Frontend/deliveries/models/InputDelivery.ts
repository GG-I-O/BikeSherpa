import {Step} from "@/steps/models/Step";

export default class InputDelivery {
    public code: string;
    public status: number;
    public customerId: string;
    public pricingStrategy: number;
    public urgency: string;
    public totalPrice: number | null;
    public discount: number | null;
    public reportId: string | null;
    public steps: Step[];
    public details: string[];
    public packingSize: string;
    public insulatedBox: boolean;
    public contractDate: string;
    public startDate: string;

    public constructor(
        code: string,
        status: number,
        customerId: string,
        pricingStrategy: number,
        urgency: string,
        totalPrice: number, 
        discount: number,
        reportId: string,
        steps: Step[],
        details: string[],
        packingSize: string,
        insulatedBox: boolean,
        contractDate: string,
        startDate: string,
    ) {
        this.code = code;
        this.status = status;
        this.customerId = customerId;
        this.pricingStrategy = pricingStrategy;
        this.urgency = urgency;
        this.totalPrice = totalPrice;
        this.discount = discount;
        this.reportId = reportId;
        this.steps = steps;
        this.details = details;
        this.packingSize = packingSize;
        this.insulatedBox = insulatedBox;
        this.contractDate = contractDate;
        this.startDate = startDate;
    }
}