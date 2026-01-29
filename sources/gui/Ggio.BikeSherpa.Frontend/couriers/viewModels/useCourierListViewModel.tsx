import { useEffect, useState } from "react";
import { observe } from "@legendapp/state";
import Courier from "../models/Courier";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { navigate } from "expo-router/build/global-state/routing";

export default function useCourierListViewModel() {
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const courierStore$ = courierServices.getCourierList$();
    const [courierList, setCourierList] = useState<Courier[]>([]);
    const [courierToDelete, setCourierToDelete] = useState<string | null>(null);

    function displayEditForm(id: string) {
        navigate({
            pathname: '/(tabs)/(couriers)/edit',
            params: { courierId: id }
        });
    }

    function deleteCourier() {
        if (courierToDelete) {
            courierServices.deleteCourier(courierToDelete);
        }
    }

    useEffect(() => {
        return observe(() => {
            const record = courierStore$.get() ?? {};
            setCourierList(Object.values(record).filter((item) => item !== undefined));
        });
    }, [courierStore$]);

    return { courierList, displayEditForm, deleteCourier, setCourierToDelete };
}