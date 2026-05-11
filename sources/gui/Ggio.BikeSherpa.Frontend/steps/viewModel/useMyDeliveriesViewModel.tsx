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
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";

export default function useMyDeliveriesViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const dropdownOptionsService = IOCContainer.get<IDropdownOptionsService>(DeliveryServiceIdentifier.DropdownOptionsService);
    const viewModel = new MyDeliveriesViewModel(deliveryServices, courierServices, customerServices, dropdownOptionsService);

    const deliveryStore$ = deliveryServices.getDeliveryList$();
    const customerStore$ = customerServices.getCustomerList$();
    const courierStore$ = courierServices.getCourierList$();
    const dropdownOptions = dropdownOptionsService.GetOptions();
    
    const [steps, setSteps] = useState<StepToDisplay[]>([]);
    const [datePicker, setDatePicker] = useState<Date|undefined>(new Date());
    
    useEffect(() => {
        viewModel.loadMyDeliveries(datePicker ?? new Date());
    }, [datePicker]);
    
    useEffect(() => {
        return observe(() => {
            setSteps(viewModel.getSteps());
        });
    }, [deliveryStore$, customerStore$, courierStore$, setSteps, dropdownOptions]);
    
    return {
        steps,
        datePicker,
        setDatePicker
    }
}