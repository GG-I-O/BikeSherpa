import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useEffect, useState} from "react";
import Delivery from "@/deliveries/models/Delivery";
import {navigate} from "expo-router/build/global-state/routing";
import {observe} from "@legendapp/state";
import DeliveryListViewModel from "@/deliveries/viewModel/DeliveryListViewModel";
import {ICourierService} from "@/spi/CourierSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {ICustomerService} from "@/spi/CustomerSPI";

export default function useDeliveryListViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new DeliveryListViewModel(deliveryServices, courierServices, customerServices);

    
    const deliveryStore$ = deliveryServices.getDeliveryList$();
    const [deliveryList, setDeliveryList] = useState<Delivery[]>([]);
    const [deliveryToDelete, setDeliveryToDelete] = useState<string | null>(null);
    
    function displayEditForm(id: string) {
        navigate({
            pathname: '/(tabs)/(deliveries)/edit',
            params: {deliveryId: id}
        });
    }
    
    function deleteDelivery() {
        if (deliveryToDelete)
            deliveryServices.deleteDelivery(deliveryToDelete);
    }
    
    useEffect(() => {
        return observe(() => {
            const record = deliveryStore$.get() ?? {};
            setDeliveryList(Object.values(record).filter((delivery) => delivery != undefined));
        });
    }, [deliveryStore$, setDeliveryList]);
    
    return {
        deliveryList,
        getFilteredDeliveries: viewModel.getFilteredDeliveries,
        getFilteredStepList: viewModel.getFilteredStepList,
        displayEditForm,
        deleteDelivery,
        setDeliveryToDelete
    };
}