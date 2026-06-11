import {useForm} from "react-hook-form";
import Customer from "../models/Customer";
import {zodResolver} from "@hookform/resolvers/zod";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {useEffect, useState} from "react";
import {observe} from "@legendapp/state";
import EditCustomerFormViewModel from "./EditCustomerFormViewModel";
import {CustomerFormValues} from "@/customers/viewModels/zod/customerFormBaseSchema";

export function useEditCustomerFormViewModel(customerId: string) {
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const customer = customerServices.getCustomer$(customerId).peek();
    const viewModel = new EditCustomerFormViewModel(customerServices);

    const customerStore$ = customerServices.getCustomerList$();
    const [customerList, setCustomerList] = useState<Customer[]>([]);

    useEffect(() => {
        return observe(() => {
            const record = customerStore$.get() ?? {};
            setCustomerList(Object.values(record));
        });
    }, [customerStore$]);

    const {
        control,
        handleSubmit,
        formState: {errors},
        setValue
    } = useForm<CustomerFormValues>({
        defaultValues: customer,
        resolver: zodResolver(viewModel.getEditCustomerSchema(customer, customerList))
    });

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                viewModel.onSubmit(customer, data);
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        setValue
    };
}