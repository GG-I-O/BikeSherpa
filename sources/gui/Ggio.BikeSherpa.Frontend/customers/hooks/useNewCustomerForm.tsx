import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as zod from 'zod';
import useCustomerViewModel from '../viewModel/CustomerViewModel';
import InputCustomer from '../models/InputCustomer';
import { addressSchema } from '@/models/Address';

const customerOptionSchema = zod.object({
    canValidateWithPhoto: zod
        .boolean(),
    canValidateWithSignature: zod
        .boolean(),
    canValidateWithFile: zod
        .boolean(),
    discount: zod
        .number(),
}).required();

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
        .max(3, "Code trop long"),
    email: zod
        .email("Adresse e-mail non valide"),
    siret: zod
        .number()
        .min(14)
        .max(14),
    comment: zod
        .string()
        .trim(),
    phone: zod
        .string()
        .trim()
        .regex(/^(?:\+33\s?[1-9]|0[1-9])(?:[\s.-]?\d{2}){4}$/, "Numéro de téléphone invalide"),
    options: customerOptionSchema
}).partial({ complement: true, siret: true, comment: true });

export function useNewCustomerForm() {
    const viewModel = useCustomerViewModel();

    const {
        control,
        handleSubmit,
        formState: { errors },
        reset
    } = useForm<InputCustomer>({
        defaultValues: {
            name: '',
            code: '',
            phone: '',
            email: '',
            address: {},
            options: {
                canValidateWithPhoto: false,
                canValidateWithSignature: false,
                canValidateWithFile: false,
                discount: 0
            }
        },
        resolver: zodResolver(newCustomerSchema)
    });

    const onSubmit = (customer: InputCustomer) => {
        viewModel.createCustomer(customer);
        reset(); // Clear form after submission
    };

    return {
        control,
        handleSubmit: handleSubmit(onSubmit),
        errors
    };
}