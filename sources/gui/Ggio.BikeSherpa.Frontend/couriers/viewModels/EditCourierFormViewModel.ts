import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Courier from "../models/Courier";
import { ICourierService } from "@/spi/CourierSPI";
import { inject } from "inversify";
import * as zod from "zod";
import {courierFormBaseSchema, CourierFormValues} from "./zod/courierFormBaseSchema";

export default class EditCourierFormViewModel {
    private courierServices: ICourierService;

    constructor(
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService
    ) {
        this.courierServices = courierServices;
    }

    onSubmit = (oldCourier: Courier, newCourier: CourierFormValues) => {
        const courier: Courier = {
            ...oldCourier,
            ...newCourier,
            address: {
                ...newCourier.address,
                name: `${newCourier.firstName} ${newCourier.lastName}`,
                phone: newCourier.phoneNumber,
            }
        }
        this.courierServices.updateCourier(courier);
    };

    public getEditCourierSchema(courierToEdit: Courier, courierList: Courier[]) {
        if (courierList.length === 0)
            return courierFormBaseSchema;
        
        const originalCode = courierToEdit.code;

        return courierFormBaseSchema
            .extend({
                code: courierFormBaseSchema.shape.code.refine((value) => {
                    if (originalCode === value) {
                        return true;
                    }
                    return !courierList.some((courier) => courier.code === value);
                }, "Le code doit être unique")
            })
    }
}