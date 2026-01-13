import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Customer from "../models/Customer";
import { addressSchema } from '@/models/Address';
import { ICustomerService } from "@/spi/CustomerSPI";
import { inject } from "inversify";
import * as zod from 'zod';

export default class EditCustomerViewModel {
    private customerServices: ICustomerService;

    constructor(
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.customerServices = customerServices;
    }

    onSubmit = (customer: Customer) => {
        this.customerServices.updateCustomer(customer);
    };

    public getEditCustomerSchema(customerToEdit: Customer, customerList: Customer[]) {
        return zod.object({
            id: zod
                .string()
                .min(1),
            name: zod
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
                    if (customerToEdit.code === value) {
                        return true;
                    }
                    return !customerList.some((customer) => customer.code === value);
                }, "Le code doit être unique"),
            email: zod
                .string()
                .email("Adresse e-mail non valide"),
            siret: zod
                .string()
                .min(14)
                .max(14).nullable(),
            phoneNumber: zod
                .string()
                .trim()
                .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide")
        }).partial({ complement: true, siret: true });
    }
}