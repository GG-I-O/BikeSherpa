import {StepToDisplay} from "@/steps/models/StepToDisplay";

export interface DeliveryToDisplay {
    id: string;
    code: string;
    customerName: string;
    urgency: string;
    steps: StepToDisplay[];
    startDate: string;
    startTime: string;
}