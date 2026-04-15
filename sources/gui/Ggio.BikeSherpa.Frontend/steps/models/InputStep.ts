import { Address } from "@/models/Address";

export default class InputStep {
    public stepType: number;
    public order: number;
    public completed: boolean;
    public stepAddress: Address;
    public stepZone: string;
    public distance: number;
    public courierId: string | null;
    public comment: string | null;
    public attachmentFilePaths: string[] | null;
    public estimatedDeliveryDate: string;
    public realDeliveryDate: string;

    public constructor(
        stepType: number,
        order: number,
        completed: boolean,
        stepAddress: Address,
        stepZone: string,
        distance: number,
        courierId: string | null,
        comment: string | null,
        attachmentFilePaths: string[] | null,
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