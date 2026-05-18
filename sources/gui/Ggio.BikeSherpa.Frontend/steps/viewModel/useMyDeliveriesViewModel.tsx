import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import MyDeliveriesViewModel from "@/steps/viewModel/MyDeliveriesViewModel";
import {useEffect, useState} from "react";
import {observe} from "@legendapp/state";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";

export default function useMyDeliveriesViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const deliveryMapper = IOCContainer.get<IDeliveryMapper>(DeliveryServiceIdentifier.Mapper);
    const viewModel = new MyDeliveriesViewModel(deliveryServices, deliveryMapper);

    const deliveryStore$ = deliveryServices.getDeliveryList$();
    
    const [steps, setSteps] = useState<StepToDisplay[]>([]);
    const [datePicker, setDatePicker] = useState<Date|undefined>(new Date());
    
    useEffect(() => {
        viewModel.loadMyDeliveries(datePicker ?? new Date());
    }, [datePicker]);
    
    useEffect(() => {
        return observe(() => {
            setSteps(viewModel.getSteps());
        });
    }, [deliveryStore$, setSteps]);
    
    return {
        steps,
        datePicker,
        setDatePicker
    }
}