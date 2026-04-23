import { Address } from '@/models/Address';
import * as Crypto from 'expo-crypto';
import InputStep from './InputStep';
import Storable from '@/models/Storable';
import { HateoasLinks, Link } from '@/models/HateoasLink';

export class Step extends InputStep implements Storable, HateoasLinks {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public operationAction?: string;
    public links?: Link[];

    public constructor(
        stepType: number,
        order: number,
        completed: boolean,
        stepAddress: Address,
        stepZone: { name: string, cities: { name: string }[] },
        distance: number,
        courierId: string,
        comment: string,
        attachmentFilePaths: string[],
        estimatedDeliveryDate: string,
        realDeliveryDate: string
    ) {
        super(
            stepType,
            order,
            completed,
            stepAddress,
            stepZone,
            distance,
            courierId,
            comment,
            attachmentFilePaths,
            estimatedDeliveryDate,
            realDeliveryDate
        );
        this.id = Crypto.randomUUID();
        this.links = [];
    }
}