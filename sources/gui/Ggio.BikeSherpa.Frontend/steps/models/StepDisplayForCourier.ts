import {Address} from "@/models/Address";

export interface StepDisplayForCourier { 
    id: string;
    deliveryId: string;
    deliveryCode: string;
    deliveryLimitDate: string;
    type: number;
    order: number;
    completed: boolean;
    address: Address;
    comment: string;
    courierComment: string;
    packing: string;
    deliveryDate: string;
    deliveryTime: string;
    estimatedIsoDate: string;
    estimatedDate: string;
    estimatedTime: string;
    distance: number;
    notBilled: boolean;
    attachmentFilePaths: string[];
}