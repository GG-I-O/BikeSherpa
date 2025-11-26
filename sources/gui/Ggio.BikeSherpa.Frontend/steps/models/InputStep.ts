import { Address } from '@/models/Address';
import { StepType } from './StepType';

export class InputStep {
    public type: StepType;
    public address: Address;
    public distance: number;
    public price: number;
    public contractDate: Date;
    public estimatedDate?: Date;
    public realDate?: Date;
    public comment?: string;
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
        comment?: string,
        nbToDo: number = 0,
        filesPath: string[] = []
    ) {
        this.type = type;
        this.address = address;
        this.distance = distance;
        this.price = price;
        this.contractDate = contractDate;
        this.estimatedDate = estimatedDate;
        this.comment = comment;

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