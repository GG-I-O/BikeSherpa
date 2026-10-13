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