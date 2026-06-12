import * as Crypto from 'expo-crypto';
import { z } from 'zod';
import { schemas } from '@/infra/openAPI/client';
import InputDelivery from "./InputDelivery";
import Storable from "@/models/Storable";
import { HateoasLinks, Link } from "@/models/HateoasLink";
import {Step} from "@/steps/models/Step";

export default class Delivery extends InputDelivery implements Storable, HateoasLinks {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public links?: Link[];
    
    public constructor(
        code: string,
        status: number,
        customerId: string,
        pricingStrategy: number,
        urgency: string,
        totalPrice: number,
        discount: number,
        extraCost: number,
        steps: Step[],
        details: string[],
        insulatedBox: boolean,
        contractDate: string,
        startDate: string,
        limitDate: string,
        needEstimate: boolean,
        customerReference: string,
    ) {
        super(
            code,
            status, 
            customerId, 
            pricingStrategy, 
            urgency,
            totalPrice,
            discount,
            extraCost,
            steps,
            details,
            insulatedBox,
            contractDate,
            startDate,
            limitDate,
            needEstimate,
            customerReference
        )
        this.id = Crypto.randomUUID();
        this.links = [];
    }
}

export type DeliveryCrud = z.infer<typeof schemas.DeliveryCrud>;

export type DeliveryDto = z.infer<typeof schemas.DeliveryDto>;