import { Address } from '@/data/Address';
import { Identifiable } from '@/data/Identifiable';
import * as Crypto from 'expo-crypto';
import { StepType } from './StepType';
import StepDetails from './StepDetails';

export class Step implements Identifiable<string> {
    readonly id: string;
    public type: StepType;
    public address: Address;
    public distance: number;
    public price: number;
    public contractDate: Date;
    public estimatedDate?: Date;
    public realDate?: Date;
    public details?: StepDetails;
    public description?: string;
    public courier?: string;
    public nbToDo: number;
    public nbDone: number;
    public filesPath: string[];

    public constructor(
        type: StepType,
        address: Address,
        distance: number,
        price: number,
        contractDate: Date,
        estimatedDate?: Date,
        details?: StepDetails,
        description?: string,
        nbToDo: number = 0,
        filesPath: string[] = []
    ) {
        this.id = Crypto.randomUUID();
        this.type = type;
        this.address = address;
        this.distance = distance;
        this.price = price;
        this.contractDate = contractDate;
        this.estimatedDate = estimatedDate;
        this.details = details;
        this.description = description;

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
        if (!this.estimatedDate) return '';
        return this.estimatedDate.toLocaleTimeString();
    }
}