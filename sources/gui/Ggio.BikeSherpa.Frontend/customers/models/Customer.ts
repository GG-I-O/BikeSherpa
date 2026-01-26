import * as Crypto from 'expo-crypto';
import InputCustomer from "./InputCustomer";
import Storable from "@/models/Storable";
import { Address, addressSchema } from "@/models/Address";
import { HateoasLinks, Link } from "@/models/HateoasLink";
import { z } from 'zod';
import { schemas } from '@/infra/openAPI/client';
import * as zod from 'zod';

export default class Customer extends InputCustomer implements Storable, HateoasLinks {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public links?: Link[];

    public constructor(
        name: string, address: Address, code: string, phone: string, email: string, siret?: string
    ) {
        super(name, address, code, phone, email, siret);
        this.id = Crypto.randomUUID();
        this.links = [];
    }
};

export type CustomerCrud = z.infer<typeof schemas.CustomerCrud>;

export type CustomerDto = z.infer<typeof schemas.CustomerDto>;

export const customerSchema = zod.object({
    name: zod
        .string()
        .trim()
        .min(1, "Nom requis"),
    address: addressSchema,
    complement: zod
        .string()
        .trim(),
    code: zod
        .string()
        .trim()
        .min(1, "Code requis")
        .max(3, "Code trop long"),
    email: zod
        .string()
        .email("Adresse e-mail non valide"),
    siret: zod
        .string()
        .min(14)
        .max(14).nullable(),
    phoneNumber: zod
        .string()
        .trim()
        .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide")
}).partial({ complement: true, siret: true });
