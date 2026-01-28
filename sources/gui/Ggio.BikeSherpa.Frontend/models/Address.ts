import * as zod from 'zod';

export type Address = {
    name: string;
    fullAddress: string;
    streetInfo: string;
    complement: string | null; 
    postcode: string;
    city: string;
}

export const addressSchema = zod.object({
    name: zod.string(),
    fullAddress: zod.string()
        .min(5, "Adresse non valide"),
    streetInfo: zod
        .string()
        .min(5, "Veuillez rentrer une adresse valide"),
    complement: zod.string().nullable(),
    postcode: zod.string(),
    city: zod.string(),
}).partial({ complement: true })