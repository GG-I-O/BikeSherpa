import * as zod from 'zod';
import { GeoPoint } from './GeoPoint';

export type Address = {
    name: string;
    fullAddress: string;
    streetInfo: string;
    complement: string | null;
    postcode: string;
    city: string;
    coordinates: GeoPoint;
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
            .nullable(),
        postcode: zod
            .string()
            .trim()
            .min(1, "Code postal requis")
            .max(5, "Code postal invalide"),
        city: zod
            .string()
            .trim()
            .min(1, "Nom de ville requis"),
        coordinates: zod
            .object({
                longitude: zod.number(),
                latitude: zod.number()
            })
            .refine(data => data.longitude >= -180 && data.longitude <= 180, {
                message: "La longitude doit être entre -180 et 180"
            })
            .refine(data => data.latitude >= -90 && data.latitude <= 90, {
                message: "La latitude doit être entre -90 et 90"
            })
    })