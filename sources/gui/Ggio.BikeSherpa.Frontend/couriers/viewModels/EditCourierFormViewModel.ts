import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Courier from "../models/Courier";
import { addressSchema } from '@/models/Address';
import { ICourierService } from "@/spi/CourierSPI";
import { inject } from "inversify";
import * as zod from 'zod';
import NewCourierFormViewModel from "./NewCourierFormViewModel";

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
        const newCourierSchema = new NewCourierFormViewModel(this.courierServices).getNewCourierSchema(courierList);

        return newCourierSchema
            .partial({ complement: true })
            .extend({
                id: zod
                    .string()
                    .min(1),
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
                    }, "Le code doit être unique")
            });
    }
}