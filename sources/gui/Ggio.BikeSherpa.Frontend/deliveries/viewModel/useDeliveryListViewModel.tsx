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
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

export default function useDeliveryListViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new DeliveryListViewModel(deliveryServices, courierServices, customerServices);

    
    const deliveryStore$ = deliveryServices.getDeliveryList$();
    const customerStore$ = customerServices.getCustomerList$();
    const courierStore$ = courierServices.getCourierList$();
    
    const [deliveryToDelete, setDeliveryToDelete] = useState<string | null>(null);
    
    const [deliveries, setDeliveries] = useState<DeliveryToDisplay[]>([]);
    const [steps, setSteps] = useState<StepToDisplay[]>([]);

    const [dateFilter, setDateFilter] = useState<string>('1');
    const [courierFilter, setCourierFilter] = useState<string>('NONE');
    
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
            setDeliveries(viewModel.getFilteredDeliveries(dateFilter, courierFilter));
            setSteps(viewModel.getFilteredStepList(dateFilter, courierFilter));
        });
        
    }, [deliveryStore$, customerStore$, courierStore$, setDeliveries, setSteps, dateFilter, courierFilter]);
    
    return {
        deliveries,
        steps,
        dateFilter,
        setDateFilter,
        courierFilter,
        setCourierFilter,
        displayEditForm,
        deleteDelivery,
        setDeliveryToDelete
    };
}