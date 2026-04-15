import * as zod from "zod";
import {addressSchema} from "@/models/Address";

export const stepFormBaseSchema = zod.object({
    stepType: zod
        .number()
        .int()
        .min(0, "Type Invalide")
        .max(1, "Type Invalide"),
    order: zod
        .number()
        .int(),
    completed: zod
        .boolean(),
    stepAddress: addressSchema,
    stepZone: zod
        .string(),
    distance: zod
        .number(),
    courierId: zod
        .string()
        .nullable(),
    comment: zod
        .string()
        .nullable(),
    attachmentFilePaths: zod
        .array(
            zod.string()
        )
        .nullable(),
    estimatedDeliveryDate: zod
        .coerce
        .string()
        .datetime({offset: true}),
    realDeliveryDate: zod
        .coerce
        .string()
        .datetime({offset: true}),
});