import { Address, addressSchema } from '@/models/Address';
import { Identifiable } from '@/models/Identifiable';
import * as Crypto from 'expo-crypto';
import { StepType } from './StepType';
import * as zod from 'zod';
import Courier, { courierSchema } from '@/couriers/models/Courier';

export class Step implements Identifiable<string> {
    readonly id: string;
    public type: StepType;
    public address: Address;
    public distance: number;
    public price: number;
    public contractDate: Date;
    public estimatedDeliveryDate?: Date;
    public realDeliveryDate?: Date;
    public comment?: string;
    public courier?: Courier;
    public nbToDo: number;
    public nbDone: number;
    public filesPath: string[];

    public constructor(
        type: StepType,
        address: Address,
        distance: number,
        price: number,
        contractDate: Date,
        estimatedDeliveryDate?: Date,
        comment?: string,
        courier?: Courier,
        nbToDo: number = 0,
        filesPath: string[] = []
    ) {
        this.id = Crypto.randomUUID();
        this.type = type;
        this.address = address;
        this.distance = distance;
        this.price = price;
        this.contractDate = contractDate;
        this.estimatedDeliveryDate = estimatedDeliveryDate;
        this.comment = comment;
        this.courier = courier;
        this.nbToDo = nbToDo;
        this.nbDone = 0;
        this.filesPath = filesPath;
    }

    public getContractDate(): string {
        return this.contractDate.toLocaleDateString();
    }

    public getContractTime(): string {
        return this.contractDate.toLocaleTimeString();
    }

    public getEstimatedTime(): string {
        if (!this.estimatedDeliveryDate) return '';
        return this.estimatedDeliveryDate.toLocaleTimeString();
    }
}

export const stepSchema = zod.object({
    id: zod.string(),
    type: zod.nativeEnum(StepType),
    addres: addressSchema,
    distance: zod.number(),
    price: zod.number(),
    contractDate: zod.date(),
    estimatedDeliveryDate: zod.date(),
    comment: zod.string(),
    courier: courierSchema,
    nbToDo: zod.number(),
    nbDone: zod.number(),
    filesPath: zod.array(zod.string()),
}).partial({ comment: true, courier: true })