import { useForm } from "react-hook-form";
import Customer from "../models/Customer";
import { zodResolver } from "@hookform/resolvers/zod";
import * as zod from 'zod';
import { addressSchema } from '@/models/Address';
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICustomerService } from "@/spi/CustomerSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { useEffect, useState } from "react";
import { observe } from "@legendapp/state";

export function useEditCustomerFormViewModel(customerId: string) {
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const customer = customerServices.getCustomer$(customerId).peek();

    const customerStore$ = customerServices.getCustomerList$();
    const [customerList, setCustomerList] = useState<Customer[]>([]);

    useEffect(() => {
        return observe(() => {
            const record = customerStore$.get() ?? {};
            setCustomerList(Object.values(record));
        });
    }, [customerStore$]);

    const editCustomerSchema = zod.object({
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
            .refine((value) => !customerList.some((customer) => customer.code === value), "Le code doit être unique"),
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

    const {
        control,
        handleSubmit,
        formState: { errors },
        setValue
    } = useForm<Customer>({
        defaultValues: customer,
        resolver: zodResolver(editCustomerSchema)
    });

    const onSubmit = (customer: Customer) => {
        customerServices.updateCustomer(customer);
    };

    return { control, handleSubmit: handleSubmit(onSubmit), errors, setValue }
}