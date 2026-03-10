import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Customer from "../models/Customer";
import { ICustomerService } from "@/spi/CustomerSPI";
import { inject } from "inversify";
import * as zod from 'zod';
import { customerFormBaseSchema } from "./zod/customerFormBaseSchema";

export default class EditCustomerFormViewModel {
    private customerServices: ICustomerService;

    constructor(
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.customerServices = customerServices;
    }

    onSubmit = (customer: Customer) => {
        customer.address.name = customer.name;
        this.customerServices.updateCustomer(customer);
    };

    public getEditCustomerSchema(customerToEdit: Customer, customerList: Customer[]) {
        const originalCode = customerToEdit.code;

        return customerFormBaseSchema
            .extend({
                id: zod
                    .string()
                    .min(1),
                code: customerFormBaseSchema.shape.code
                    .refine((value) => {
                        if (originalCode === value) {
                            return true;
                        }
                        return !customerList.some((customer) => customer.code === value);
                    }, "Le code doit être unique"),
            });
    }
}