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
            .positive()
            .nullable(),
        discount: zod
            .number()
            .positive()
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
            .string()
            .datetime({ offset: true }),
        startDate: zod
            .string()
            .datetime({ offset: true }),
    });