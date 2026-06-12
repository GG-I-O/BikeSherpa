import * as zod from "zod";
import {stepFormBaseSchema} from "@/steps/models/zod/stepFormBaseSchema";
import {addressSchema} from "@/models/Address";

const publicDeliveryFormBaseSchema = zod
    .object({
        pricingStrategy: zod
            .number()
            .int("Type invalide")
            .gte(0, "Type invalide")
            .lte(2, "Type invalide"),
        urgency: zod
            .string()
            .trim(),
        totalPrice: zod
            .number()
            .gte(0, "Le prix ne peut pas être négatif")
            .nullable(),
        needEstimate: zod
            .boolean(),
        customer: zod
            .object({
                code: zod
                    .string()
                    .trim()
                    .min(1, "Code requis"),
                name: zod
                    .string()
                    .trim()
                    .min(1, "Nom requis"),
                email: zod
                    .string()
                    .min(1, "Adresse e-mail requise")
                    .email("Adresse e-mail invalide"),
                phoneNumber: zod
                    .string()
                    .trim()
                    .min(1, "Numéro de téléphone obligatoire")
                    .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide")
                    .nullable(),
                address: addressSchema
            }),
        steps: zod
            .array(stepFormBaseSchema),
        insulatedBox: zod
            .boolean(),
        startDate: zod
            .coerce
            .string()
            .datetime({offset: true}),
    });

type PublicDeliveryFormValues = zod.infer<typeof publicDeliveryFormBaseSchema>;

export {publicDeliveryFormBaseSchema, PublicDeliveryFormValues};