import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Courier from "../models/Courier";
import { addressSchema } from '@/models/Address';
import { ICourierService } from "@/spi/CourierSPI";
import { inject } from "inversify";
import * as zod from 'zod';

export default class EditCourierFormViewModel {
    private courierServices: ICourierService;

    constructor(
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService
    ) {
        this.courierServices = courierServices;
    }

    onSubmit = (courier: Courier) => {
        courier.address.name = `${courier.firstName} ${courier.lastName}`;
        this.courierServices.updateCourier(courier);
    };

    public getEditCourierSchema(courierToEdit: Courier, courierList: Courier[]) {
        const originalCode = courierToEdit.code;
        return zod.object({
            id: zod
                .string()
                .min(1),
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
                .trim(),
            code: zod
                .string()
                .trim()
                .min(1, "Code requis")
                .max(3, "Code trop long")
                .refine((value) => {
                    if (originalCode === value) {
                        return true;
                    }
                    return !courierList.some((courier) => courier.code === value);
                }, "Le code doit être unique"),
            email: zod
                .string()
                .email("Adresse e-mail non valide"),
            phoneNumber: zod
                .string()
                .trim()
                .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide")
        }).partial({ complement: true });
    }
}