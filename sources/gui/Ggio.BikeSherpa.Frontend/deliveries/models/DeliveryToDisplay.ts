import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {DeliveryStatusEnum} from "@/deliveries/data/deliveryStatusEnum";

export interface DeliveryToDisplay {
    id: string;
    code: string;
    status: DeliveryStatusEnum;
    customerName: string;
    urgency: string;
    steps: StepToDisplay[];
    totalPrice: number;
    deliveryInfo: string;
    startDate: string;
    startTime: string;
    limitTime: string;
}