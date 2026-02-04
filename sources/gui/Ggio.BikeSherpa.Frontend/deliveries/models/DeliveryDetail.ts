import * as zod from "zod";

export default class DeliveryDetail {
    public label: string;
    public price: number;

    constructor(label: string, price: number) {
        this.label = label;
        this.price = price;
    }
}

export const detailSchema = zod.object({
    label: zod.string(),
    price: zod.number(),
});

export const detailSchema = zod.object({
    label: zod.string(),
    price: zod.number(),
});