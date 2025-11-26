import { Address } from '@/models/Address';
import * as Crypto from 'expo-crypto';
import { StepType } from './StepType';
import { InputStep } from './InputStep';
import Storable from '@/models/Storable';

export class Step extends InputStep implements Storable {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;

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
        super(type, address, distance, price, contractDate, estimatedDate, comment, nbToDo, filesPath);
        this.id = Crypto.randomUUID();
    }
}