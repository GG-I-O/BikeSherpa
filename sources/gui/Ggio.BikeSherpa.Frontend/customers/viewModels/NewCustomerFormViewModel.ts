import InputCustomer from "../models/InputCustomer";
import { ICustomerService } from "@/spi/CustomerSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import Customer from "../models/Customer";
import { customerFormBaseSchema } from "./zod/customerFormBaseSchema";

export default class NewCustomerFormViewModel {
    private customerServices: ICustomerService;
    private resetCallback?: UseFormReset<InputCustomer>;

    constructor(
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.customerServices = customerServices;
    }

    public onSubmit = (customer: InputCustomer): void => {
        customer.address.name = customer.name;
        this.customerServices.createCustomer(customer);
        if (this.resetCallback) {
            this.resetCallback(); // Clear form after submission
        }
    };

    public setResetCallback(reset?: UseFormReset<InputCustomer>) {
        this.resetCallback = reset;
    }

    public getNewCustomerSchema(customerList: Customer[]) {
        return customerFormBaseSchema.extend({
            code: customerFormBaseSchema.shape.code.refine(
                (value) => !customerList.some((customer) => customer.code === value),
                "Le code doit être unique"
            ),
        })
    }
}