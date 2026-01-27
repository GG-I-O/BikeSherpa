import { Identifiable } from "@/models/Identifiable";
import * as Crypto from 'expo-crypto';
import { Step } from "@/steps/models/Step";
import Customer from "@/customers/models/Customer";
import DeliveryDetail from "./DeliveryDetail";
import { DeliveryPacking } from "./DeliveryPacking";
import { InputDelivery } from "./InputDelivery";
import { Link } from "@/models/HateoasLink";
import { z } from 'zod';
import { schemas } from '@/infra/openAPI/client';

export class Delivery extends InputDelivery implements Identifiable<string> {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public links?: Link[];

    public constructor(code: string, customer: Customer, totalPrice: number, reportId: string, steps: Step[] = [], details: DeliveryDetail[] = [], packing: DeliveryPacking) {
        super(code, customer, totalPrice, reportId, steps, details, packing);
        this.id = Crypto.randomUUID();
        this.links = [];
    }
}

export type DeliveryCrud = z.infer<typeof schemas.DeliveryCrud>;

export type DeliveryDto = z.infer<typeof schemas.DeliveryDto>;