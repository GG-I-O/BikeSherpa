import { Address } from "@/models/Address";

export default class InputStep {
    public stepType: number;
    public order: number;
    public completed: boolean;
    public stepAddress: Address;
    public stepZone: string;
    public distance: number;
    public courierId: string;
    public comment: string;
    public attachmentFilePaths: string[];
    public estimatedDeliveryDate: string;
    public realDeliveryDate: string;

    public constructor(
        stepType: number,
        order: number,
        completed: boolean,
        stepAddress: Address,
        stepZone: string,
        distance: number,
        courierId: string,
        comment: string,
        attachmentFilePaths: string[],
        estimatedDeliveryDate: string,
        realDeliveryDate: string
    ) {
        this.stepType = stepType;
        this.order = order;
        this.completed = completed;
        this.stepAddress = stepAddress;
        this.stepZone = stepZone;
        this.distance = distance;
        this.courierId = courierId;
        this.comment = comment;
        this.attachmentFilePaths = attachmentFilePaths;
        this.estimatedDeliveryDate = estimatedDeliveryDate;
        this.realDeliveryDate = realDeliveryDate;
    }
}