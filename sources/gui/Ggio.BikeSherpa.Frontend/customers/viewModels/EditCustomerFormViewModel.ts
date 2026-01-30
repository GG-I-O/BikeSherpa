import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Customer from "../models/Customer";
import { ICustomerService } from "@/spi/CustomerSPI";
import { inject } from "inversify";
import * as zod from 'zod';
import NewCustomerFormViewModel from "./NewCustomerFormViewModel";

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
        const newCustomerViewModel = new NewCustomerFormViewModel(this.customerServices);
        const getEditCustomerSchema = newCustomerViewModel.getNewCustomerSchema(customerList);
        return zod.object({
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
                    return !customerList.some((customer) => customer.code === value);
                }, "Le code doit être unique"),
            name: getEditCustomerSchema.shape.name,
            address: getEditCustomerSchema.shape.address,
            vatNumer: getEditCustomerSchema.shape.vatNumber,
            siret: getEditCustomerSchema.shape.siret,
            email: getEditCustomerSchema.shape.email,
            phoneNumber: getEditCustomerSchema.shape.phoneNumber
        })
    }
}