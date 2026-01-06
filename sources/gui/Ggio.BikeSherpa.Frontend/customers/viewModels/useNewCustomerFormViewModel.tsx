import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as zod from 'zod';
import InputCustomer from '../models/InputCustomer';
import { addressSchema } from '@/models/Address';
import { ICustomerService } from '@/spi/CustomerSPI';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { useEffect, useState } from 'react';
import Customer from '../models/Customer';
import { observe } from '@legendapp/state';

export function useNewCustomerFormViewModel() {
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const customerStore$ = customerServices.getCustomerList$();
    const [customerList, setCustomerList] = useState<Customer[]>([]);

    useEffect(() => {
        return observe(() => {
            const record = customerStore$.get() ?? {};
            setCustomerList(Object.values(record));
        });
    }, [customerStore$]);

    const newCustomerSchema = zod.object({
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
        reset
    } = useForm<InputCustomer>({
        defaultValues: {
            name: '',
            code: '',
            phoneNumber: '',
            email: '',
            address: {
                name: '',
                streetInfo: '',
                complement: '',
                postcode: '',
                city: ''
            },
        },
        resolver: zodResolver(newCustomerSchema)
    });

    const onSubmit = (customer: InputCustomer) => {
        customer.address.name = customer.name;
        try {
            customerServices.createCustomer(customer);
            reset(); // Clear form after submission
        } catch (error) {
            console.log("toto est mort");
        }
    };

    return {
        control,
        handleSubmit: handleSubmit(onSubmit),
        errors
    };
}