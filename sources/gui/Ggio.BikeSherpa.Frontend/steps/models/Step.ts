import { Address } from '@/data/Address';
import { Identifiable } from '@/data/Identifiable';
import DateToolbox from '@/services/DateToolbox';
import * as Crypto from 'expo-crypto';
import { StepType } from './StepType';

export class Step implements Identifiable<string> {
    readonly id: string;
    public deliveryCode: string;
    public address: Address;
    public type: StepType;
    public contractDate: Date;
    public comment?: string;
    public description?: string;
    public courier?: string;
    public estimatedDate?: Date;
    public realDate?: Date;

    public constructor(
        deliveryCode: string,
        address: Address,
        type: StepType,
        contractDate: Date,
        estimatedDate?: Date,
        comment?: string,
        description?: string
    ) {
        this.id = Crypto.randomUUID();
        this.deliveryCode = deliveryCode;
        this.address = address;
        this.type = type;
        this.contractDate = contractDate;
        this.estimatedDate = estimatedDate;
        this.comment = comment;
        this.description = description;
    }

    public getContractDate(): string {
        return DateToolbox.formatDate(this.contractDate);
    }

    public getContractTime(): string {
        return DateToolbox.formatTime(this.contractDate);
    }

    public getEstimatedTime(): string {
        if (!this.estimatedDate) return '';
        return DateToolbox.formatTime(this.estimatedDate);
    }

    // Parse from Json
    static fromPlainObject(obj: any): Step {
        const step = Object.create(Step.prototype);
        Object.assign(step, {
            ...obj,
            contractDate: new Date(obj.contractDate),
            estmatedDate: obj.estmatedDate ? new Date(obj.estmatedDate) : undefined
        });
        return step;
    }
}