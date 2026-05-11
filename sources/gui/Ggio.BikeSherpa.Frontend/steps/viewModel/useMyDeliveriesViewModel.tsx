import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ICourierService} from "@/spi/CourierSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {ICustomerService} from "@/spi/CustomerSPI";
import MyDeliveriesViewModel from "@/steps/viewModel/MyDeliveriesViewModel";
import {useEffect, useState} from "react";
import {observe} from "@legendapp/state";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

export default function useMyDeliveriesViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new MyDeliveriesViewModel(deliveryServices, courierServices, customerServices);

    const deliveryStore$ = deliveryServices.getDeliveryList$();
    const customerStore$ = customerServices.getCustomerList$();
    const courierStore$ = courierServices.getCourierList$();
    
    const [steps, setSteps] = useState<StepToDisplay[]>([]);
    const [datePicker, setDatePicker] = useState<Date|undefined>(new Date());
    
    useEffect(() => {
        viewModel.loadMyDeliveries(datePicker ?? new Date());
    }, [datePicker]);
    
    useEffect(() => {
        return observe(() => {
            setSteps(viewModel.getSteps());
        });
    }, [deliveryStore$, customerStore$, courierStore$, setSteps]);
    
    return {
        steps,
        datePicker,
        setDatePicker
    }
}