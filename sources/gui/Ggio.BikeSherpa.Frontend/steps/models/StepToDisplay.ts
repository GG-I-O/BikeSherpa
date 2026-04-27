import {Address} from "@/models/Address";


export interface StepToDisplay { 
    id: string;
    deliveryId: string;
    deliveryCode: string;
    deliveryUrgency: string;
    type: number;
    order: number;
    completed: boolean;
    address: Address;
    courierCode?: string;
    comment: string;
    deliveryDate: string;
    deliveryTime: string;
    estimatedIsoDate: string;
    estimatedDate: string;
    estimatedTime: string;
}