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
        .nullable()
});

type StepFormValues = zod.infer<typeof stepFormBaseSchema>;

export {stepFormBaseSchema, StepFormValues};