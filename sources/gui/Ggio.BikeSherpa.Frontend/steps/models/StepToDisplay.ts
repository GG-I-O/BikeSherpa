import {Address} from "@/models/Address";


export interface StepToDisplay { 
    id: string;
    type: number;
    order: number;
    completed: boolean;
    address: Address;
    courierCode?: string;
    comment: string;
    estimatedDate: string;
    estimatedTime: string;
}