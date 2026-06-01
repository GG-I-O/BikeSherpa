import * as zod from "zod";

const deliveryCustomerSchema = zod
    .object({
        email: zod.string().trim().min(1, "Email obligatoire").email("Email au mauvais format"),
        code: zod.string().trim().min(1, "Code obligatoire"),
    });

type DeliveryCustomerValues = zod.infer<typeof deliveryCustomerSchema>;

export {deliveryCustomerSchema, DeliveryCustomerValues};