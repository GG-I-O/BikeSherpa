import InputCourier from "../models/InputCourier";
import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import Courier from "../models/Courier";
import { courierFormBaseSchema, getCourierFormSchemaPartial } from "./courierFormZodSchema/courierFormZodSchema";

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

    public getNewCourierSchema(courierList: Courier[]) {
        return courierFormBaseSchema.extend({
            code: courierFormBaseSchema.shape.code.refine(
                (value) => !courierList.some((c) => c.code === value),
                "Le code doit être unique"
            ),
        })
            .partial(getCourierFormSchemaPartial());
    }
}