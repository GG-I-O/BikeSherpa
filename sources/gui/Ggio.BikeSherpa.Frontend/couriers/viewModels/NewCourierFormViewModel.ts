import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import Courier from "../models/Courier";
import { courierFormBaseSchema, CourierFormValues } from "./zod/courierFormBaseSchema";
import CourierMapper from "@/couriers/services/CourierMapper";

export default class NewCourierFormViewModel {
    private courierServices: ICourierService;
    private resetCallback?: UseFormReset<CourierFormValues>;

    constructor(
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService
    ) {
        this.courierServices = courierServices;
    }

    public onSubmit = (courier: CourierFormValues): void => {
        const inputCourier = CourierMapper.CourierFormValuesToInputCourier(courier);
        this.courierServices.createCourier(inputCourier);
        if (this.resetCallback) {
            this.resetCallback(); // Clear form after submission
        }
    };

    public setResetCallback(reset?: UseFormReset<CourierFormValues>) {
        this.resetCallback = reset;
    }

    public getNewCourierSchema(courierList: Courier[]) {
        return courierFormBaseSchema.extend({
            code: courierFormBaseSchema.shape.code.refine(
                (value) => !courierList.some((c) => c.code === value),
                "Le code doit être unique"
            ),
        })
    }
}