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
    public links?: Link[];

    public constructor(
        stepType: number,
        order: number,
        completed: boolean,
        stepAddress: Address,
        stepZone: string,
        packingSize: string,
        distance: number,
        courierId: string,
        comment: string,
        courierComment: string,
        notBilled: boolean,
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
            packingSize,
            distance,
            courierId,
            comment,
            courierComment,
            notBilled,
            attachmentFilePaths,
            estimatedDeliveryDate,
            realDeliveryDate
        );
        this.id = Crypto.randomUUID();
        this.links = [];
    }
}