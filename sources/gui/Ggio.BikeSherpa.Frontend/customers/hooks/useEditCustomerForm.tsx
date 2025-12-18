import { useForm } from "react-hook-form";
import Customer from "../models/Customer";
import { zodResolver } from "@hookform/resolvers/zod";
import useCustomerViewModel from "../viewModel/CustomerViewModel";
import * as zod from 'zod';
import { addressSchema } from '@/models/Address';

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
        .max(3, "Code trop long"),
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

export function useEditCustomerForm() {
    const viewModel = useCustomerViewModel();

    const {
        control,
        handleSubmit,
        formState: { errors },
        setValue
    } = useForm<Customer>({
        defaultValues: {
            id: '',
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
        resolver: zodResolver(editCustomerSchema)
    });

    const onSubmit = (customer: Customer) => {
        viewModel.updateCustomer(customer);
    };

    return { control, handleSubmit: handleSubmit(onSubmit), errors, setValue }
}