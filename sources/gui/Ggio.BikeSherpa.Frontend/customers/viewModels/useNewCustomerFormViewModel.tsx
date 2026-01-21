import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import InputCustomer from '../models/InputCustomer';
import { ICustomerService } from '@/spi/CustomerSPI';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { useEffect, useState } from 'react';
import Customer from '../models/Customer';
import { observe } from '@legendapp/state';
import NewCustomerFormViewModel from './NewCustomerFormViewModel';

export function useNewCustomerFormViewModel() {
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const customerStore$ = customerServices.getCustomerList$();
    const [customerList, setCustomerList] = useState<Customer[]>([]);
    const newCustomerViewModel = new NewCustomerFormViewModel(customerServices);

    useEffect(() => {
        return observe(() => {
            const record = customerStore$.get() ?? {};
            setCustomerList(Object.values(record));
        });
    }, [customerStore$]);

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
        resolver: zodResolver(newCustomerViewModel.getNewCustomerSchema(customerList))
    });

    newCustomerViewModel.setResetCallback(reset);

    return {
        control,
        handleSubmit: handleSubmit(newCustomerViewModel.onSubmit),
        errors
    };
}