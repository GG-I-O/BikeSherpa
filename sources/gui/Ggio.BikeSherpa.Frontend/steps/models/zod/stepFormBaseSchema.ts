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
    packingSize: zod
        .string()
        .trim(),
    contactName: zod
        .string()
        .trim()
        .min(1, "Nom requis"),
    contactPhone: zod
        .string()
        .trim()
});

type StepFormValues = zod.infer<typeof stepFormBaseSchema>;

export {stepFormBaseSchema, StepFormValues};