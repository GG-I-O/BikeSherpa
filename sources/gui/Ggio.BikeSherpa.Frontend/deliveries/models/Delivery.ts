import { Identifiable } from "@/data/Identifiable";
import * as Crypto from 'expo-crypto';
import { Step } from "@/steps/models/Step";

export class Delivery implements Identifiable<string> {
    readonly id: string;
    readonly code: string;
    readonly customer: string;
    readonly steps?: Array<Step>;

    public constructor(code: string, customer: string) {
        this.id = Crypto.randomUUID();
        this.code = code;
        this.customer = customer;
    }

    // Parse from json
    static fromPlainObject(obj: any): Delivery {
        const delivery = Object.create(Delivery.prototype);
        Object.assign(delivery, obj);

        if (obj.steps) {
            delivery.steps = obj.steps.map((e: any) => Step.fromPlainObject(e));
        }

        return delivery;
    }
}