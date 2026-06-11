import * as zod from "zod";
import { addressSchema } from "@/models/Address";

const customerFormBaseSchema = zod
    .object({
        name: zod
            .string()
            .trim()
            .min(1, "Nom requis"),
        address: addressSchema,
        code: zod
            .string()
            .trim()
            .min(1, "Code requis"),
        email: zod
            .string()
            .min(1, "Adresse e-mail requise")
            .email("Adresse e-mail invalide"),
        siret: zod
            .string()
            .transform(val => val === "" ? null : val)
            .nullable()
            .refine(val => val === null || val.length === 14, { message: "Siret invalide" }),
        vatNumber: zod
            .string()
            .transform(val => val === "" ? null : val)
            .nullable()
            .refine(val => val === null || val.length === 13, { message: "Numéro de TVA invalide" }),
        phoneNumber: zod
            .string()
            .trim()
            .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide"),
        defaultDeliveryType: zod
            .number()
    });

type CustomerFormValues = zod.infer<typeof customerFormBaseSchema>;

export {customerFormBaseSchema, CustomerFormValues}
