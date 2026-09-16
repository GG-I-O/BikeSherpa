import {DeliveryStatusEnum} from "@/deliveries/data/deliveryStatusEnum";
import {StepDisplayForCourier} from "@/steps/models/StepDisplayForCourier";

export interface DeliveryDisplayForCourier {
    id: string;
    code: string;
    status: DeliveryStatusEnum;
    customerName: string;
    urgency: string;
    steps: StepDisplayForCourier[];
    totalPrice: number;
    startDate: string;
    startTime: string;
    limitTime: string;
}