import * as zod from "zod";

export const deliveryFormBaseSchema = zod
    .object({
        code: zod
            .string()
            .trim(),
        status: zod
            .number()
            .int("Statut invalide")
            .gte(0, "Statut invalide")
            .lte(3, "Statut invalide"),
        customerId: zod
            .string()
            .trim(),
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
        discount: zod
            .number()
            .gte(0, "La remise ne peut pas être négative")
            .nullable(),
        reportId: zod
            .string()
            .nullable(),
        details: zod
            .array(zod.string()),
        packingSize: zod
            .string()
            .trim(),
        insulatedBox: zod
            .boolean(),
        contractDate: zod
            .coerce
            .string()
            .datetime({offset: true}),
        startDate: zod
            .coerce
            .string()
            .datetime({offset: true}),
    });