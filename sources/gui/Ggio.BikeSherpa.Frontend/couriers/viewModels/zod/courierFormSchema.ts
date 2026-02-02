import * as zod from 'zod';
import { addressSchema } from '@/models/Address';

type CourierFormSchema = zod.infer<typeof courierFormBaseSchema>;

type CourierFormPartialShape = Partial<Record<keyof CourierFormSchema, true>>;

export const courierFormBaseSchema = zod
    .object({
        firstName: zod
            .string()
            .trim()
            .min(1, "Prénom requis")
            .max(50, "Prénom trop long"),
        lastName: zod
            .string()
            .trim()
            .min(1, "Nom requis")
            .max(50, "Nom trop long"),
        address: addressSchema,
        complement: zod
            .string()
            .nullable(),
        code: zod
            .string()
            .trim()
            .min(1, "Code requis")
            .max(3, "Code trop long"),
        email: zod
            .string()
            .trim()
            .min(1, "Adresse e-mail requise")
            .email("Adresse e-mail invalide"),
        phoneNumber: zod
            .string()
            .trim()
            .min(1, "Numéro de téléphone requis")
            .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide"),
    });

export function getCourierFormSchemaPartial(): CourierFormPartialShape {
    return { complement: true };
}