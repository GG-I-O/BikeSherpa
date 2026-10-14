import * as zod from "zod";
import {addressSchema} from "@/models/Address";

const publicDeliveryCustomerFormBaseSchema = zod
    .object({
        code: zod
            .string()
            .trim()
            .min(1, "Code requis"),
        name: zod
            .string()
            .trim(),
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
        needEstimate: zod
            .boolean(),
        address: addressSchema
    });

type PublicDeliveryCustomerFormValues = zod.infer<typeof publicDeliveryCustomerFormBaseSchema>;

export {publicDeliveryCustomerFormBaseSchema, PublicDeliveryCustomerFormValues};