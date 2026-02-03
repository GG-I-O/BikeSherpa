import * as zod from 'zod';

export type Address = {
    name: string;
    fullAddress: string;
    streetInfo: string;    
    complement?: string | null;
    postcode: string;
    city: string;
}

export const addressSchema = zod
    .object({
        name: zod
            .string(),
        fullAddress: zod
            .string()
            .trim()
            .min(1, "Adresse requise")
            .min(5, "Adresse non valide"),
        streetInfo: zod
            .string()
            .min(5, "Veuillez rentrer une adresse valide"),
        complement: zod
            .string()
            .nullable()
            .optional(),
        postcode: zod
            .string()
            .trim()
            .min(1, "Code postal requis")
            .max(5, "Code postal invalide"),
        city: zod
            .string()
            .trim()
            .min(1, "Nom de ville requis"),
    })