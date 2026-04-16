import {Address} from "@/models/Address";


export interface StepToDisplay { 
    id: string;
    type: number;
    completed: boolean;
    address: Address;
    courierCode?: string;
    comment: string;
    estimatedDate: string;
    estimatedTime: string;
}