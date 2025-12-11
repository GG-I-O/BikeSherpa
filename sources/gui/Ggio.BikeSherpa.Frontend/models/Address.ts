import * as zod from 'zod';

export type Address = {
    name: string;
    streetInfo: string;
    complement?: string | null;
    postcode: string;
    city: string;
}

export const addressSchema = zod.object({
    name: zod.string(),
    streetInfo: zod
    .string()
    .min(5, "Veuillez rentrer une adresse valide"),
    complement: zod.string(),
    postcode: zod.string(),
    city: zod.string(),
}).partial({ complement: true })