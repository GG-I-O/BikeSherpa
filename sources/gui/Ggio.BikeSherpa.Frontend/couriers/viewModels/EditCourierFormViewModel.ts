import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Courier from "../models/Courier";
import { ICourierService } from "@/spi/CourierSPI";
import { inject } from "inversify";
import * as zod from 'zod';
import { courierFormBaseSchema, getCourierFormSchemaPartial } from "./zod/courierFormSchema";

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

        return courierFormBaseSchema
            .extend({
                id: zod
                    .string()
                    .min(1),
                code: courierFormBaseSchema.shape.code.refine((value) => {
                    if (originalCode === value) {
                        return true;
                    }
                    return !courierList.some((courier) => courier.code === value);
                }, "Le code doit être unique")
            })
            .partial(getCourierFormSchemaPartial());
    }
}