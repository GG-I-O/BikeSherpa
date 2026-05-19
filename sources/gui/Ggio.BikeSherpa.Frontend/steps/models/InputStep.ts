import {Address} from "@/models/Address";

export default class InputStep {
    public stepType: number;
    public order: number;
    public completed: boolean;
    public stepAddress: Address;
    public stepZone: { name: string, cities: { name: string }[] };
    public distance: number;
    public courierId: string | null;
    public comment: string | null;
    public courierComment: string | null;
    public notBilled: boolean;
    public attachmentFilePaths: string[] | null;
    public estimatedDeliveryDate: string;
    public realDeliveryDate: string | null;

    public constructor(
        stepType: number,
        order: number,
        completed: boolean,
        stepAddress: Address,
        stepZone: { name: string, cities: { name: string }[] },
        distance: number,
        courierId: string | null,
        comment: string | null,
        courierComment: string | null,
        notBilled: boolean,
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
        this.courierComment = courierComment;
        this.notBilled = notBilled;
        this.attachmentFilePaths = attachmentFilePaths;
        this.estimatedDeliveryDate = estimatedDeliveryDate;
        this.realDeliveryDate = realDeliveryDate;
    }
}