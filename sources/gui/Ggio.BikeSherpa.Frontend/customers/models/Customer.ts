import * as Crypto from 'expo-crypto';
import InputCustomer from "./InputCustomer";
import Storable from "@/models/Storable";
import { Address } from "@/models/Address";
import { HateoasLinks, Link } from "@/models/HateoasLink";
import { z } from 'zod';
import { schemas } from '@/infra/openAPI/client';

export default class Customer extends InputCustomer implements Storable, HateoasLinks {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public links?: Link[];

    public constructor(
        name: string, address: Address, code: string, phone: string, email: string, siret?: string, vatNumber?: string
    ) {
        super(name, address, code, phone, email, siret, vatNumber);
        this.id = Crypto.randomUUID();
        this.links = [];
    }
};

export type CustomerCrud = z.infer<typeof schemas.CustomerCrud>;

export type CustomerDto = z.infer<typeof schemas.CustomerDto>;
