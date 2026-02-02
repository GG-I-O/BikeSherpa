import InputCourier from "../models/InputCourier";
import * as zod from 'zod';
import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import { addressSchema } from '@/models/Address';
import Courier from "../models/Courier";

const newCourierBaseSchema = zod.object({
    firstName: zod
        .string()
        .trim()
        .min(1, "Prénom requis"),
    lastName: zod
        .string()
        .trim()
        .min(1, "Nom requis"),
    address: addressSchema,
    complement: zod
        .string()
        .nullable(),
    code: zod
        .string()
        .trim()
        .min(1, "Code requis")
        .max(3, "Code trop long"),
    email: zod
        .string()
        .trim()
        .min(1, "Adresse e-mail requise")
        .email(),
    phoneNumber: zod
        .string()
        .trim()
        .min(1, "Numéro de téléphone requis")
        .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide"),
});

type NewCourierSchema = zod.infer<typeof newCourierBaseSchema>;

type NewCourierPartialShape = Partial<Record<keyof NewCourierSchema, true>>;

export default class NewCourierFormViewModel {
    private courierServices: ICourierService;
    private resetCallback?: UseFormReset<InputCourier>;

    constructor(
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService
    ) {
        this.courierServices = courierServices;
    }

    public onSubmit = (courier: InputCourier): void => {
        courier.address.name = `${courier.firstName} ${courier.lastName}`;
        this.courierServices.createCourier(courier);
        if (this.resetCallback) {
            this.resetCallback(); // Clear form after submission
        }
    };

    public setResetCallback(reset?: UseFormReset<InputCourier>) {
        this.resetCallback = reset;
    }

    public getNewCourierSchemaPartial(): NewCourierPartialShape {
        return { complement: true };
    }

    public getNewCourierSchema(courierList: Courier[]) {
        return newCourierBaseSchema.extend({
            code: newCourierBaseSchema.shape.code.refine(
                (value) => !courierList.some((c) => c.code === value),
                "Le code doit être unique"
            ),
        })
            .partial(this.getNewCourierSchemaPartial());
    }
}