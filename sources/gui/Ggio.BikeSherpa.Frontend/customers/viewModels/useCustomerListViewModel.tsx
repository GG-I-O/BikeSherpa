import { useEffect, useState } from "react";
import CustomerServices from "../services/CustomerServices";
import { observe } from "@legendapp/state";
import Customer from "../models/Customer";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICustomerService } from "@/spi/CustomerSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { navigate } from "expo-router/build/global-state/routing";

export default function useCustomerListViewModel() {
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const customerStore$ = customerServices.getCustomerList$();
    const [customerList, setCustomerList] = useState<Customer[]>([]);

    function displayEditForm(id: string) {
        navigate({
            pathname: '/(tabs)/(customers)/edit',
            params: { customerId: id }
        });
    }

    useEffect(() => {
        return observe(() => {
            const record = customerStore$.get() ?? {};
            setCustomerList(Object.values(record));
        });
    }, [customerStore$]);

    return { customerList, displayEditForm };
}