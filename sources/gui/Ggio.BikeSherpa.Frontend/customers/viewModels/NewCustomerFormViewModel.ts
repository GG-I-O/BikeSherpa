import InputCustomer from "../models/InputCustomer";
import * as zod from 'zod';
import { ICustomerService } from "@/spi/CustomerSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import { addressSchema } from '@/models/Address';
import Customer from "../models/Customer";

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
        return zod.object({
            name: zod
                .string()
                .trim()
                .min(1, "Nom requis"),
            address: addressSchema,
            complement: zod
                .string()
                .nullable(),
            code: zod
                .string()
                .trim()
                .min(1, "Code requis")
                .max(3, "Code trop long")
                .refine((value) => !customerList.some((customer) => customer.code === value), "Le code doit être unique"),
            email: zod
                .string()
                .min(1, "Adresse e-mail requise")
                .email("Adresse e-mail invalide"),
            siret: zod.string()
                .transform(val => val === "" ? null : val)
                .nullable()
                .refine(val => val === null || val.length === 14, { message: "Siret invalide" }),
            vatNumber: zod.string()
                .transform(val => val === "" ? null : val)
                .nullable()
                .refine(val => val === null || val.length === 13, { message: "Numéro de TVA invalide" }),
            phoneNumber: zod
                .string()
                .trim()
                .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide")
        }).partial({ complement: true, siret: true, vatNumber: true });
    }
}