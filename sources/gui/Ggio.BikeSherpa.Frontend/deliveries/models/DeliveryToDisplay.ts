import {StepToDisplay} from "@/steps/models/StepToDisplay";

export interface DeliveryToDisplay {
    id: string;
    code: string;
    customerName: string;
    urgency: string;
    steps: StepToDisplay[];
    totalPrice: number;
    startDate: string;
    startTime: string;
    limitTime: string;
}