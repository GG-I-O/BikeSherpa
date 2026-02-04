import * as zod from "zod";
import { customerSchema } from "@/customers/models/Customer";
import { stepSchema } from "@/steps/models/Step";
import { detailSchema } from "@/deliveries/models/DeliveryDetail";
import { DeliveryPacking } from "@/deliveries/models/DeliveryPacking";

export const deliveryFormBaseSchema = zod
    .object({
        code: zod
            .string()
            .trim()
            .min(1, "Code requis")
            .max(3, "Code trop long"),
        customer: customerSchema,
        totalPrice: zod
            .number(),
        reportId: zod
            .string(),
        steps: zod.array(stepSchema),
        details: zod.array(detailSchema),
        packing: zod.nativeEnum(DeliveryPacking),
    });