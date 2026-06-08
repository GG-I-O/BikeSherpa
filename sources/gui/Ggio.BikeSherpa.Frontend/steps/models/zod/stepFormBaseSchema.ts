import * as zod from "zod";
import {addressSchema} from "@/models/Address";

const stepFormBaseSchema = zod.object({
    id: zod
        .string()
        .optional(),
    stepType: zod
        .number()
        .int()
        .min(0, "Type Invalide")
        .max(1, "Type Invalide"),
    stepAddress: addressSchema,
    comment: zod
        .string()
        .optional()
        .nullable(),
    courierComment: zod
        .string()
        .optional()
        .nullable(),
    notBilled: zod
        .boolean(),
    contactName: zod
        .string()
        .trim()
        .min(1, "Nom requis"),
    contactPhone: zod
        .string()
        .trim()
        .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide")
});

type StepFormValues = zod.infer<typeof stepFormBaseSchema>;

export {stepFormBaseSchema, StepFormValues};