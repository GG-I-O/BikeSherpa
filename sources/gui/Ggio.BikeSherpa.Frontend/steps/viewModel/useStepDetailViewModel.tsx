import {useEffect, useState} from "react";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ICourierService} from "@/spi/CourierSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import StepDetailViewModel from "@/steps/viewModel/StepDetailViewModel";
import {observe} from "@legendapp/state";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";

export default function useStepDetailViewModel(stepId: string) {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const dropdownOptionsService = IOCContainer.get<IDropdownOptionsService>(DeliveryServiceIdentifier.DropdownOptionsService);
    const viewModel = new StepDetailViewModel(deliveryServices, courierServices, dropdownOptionsService);

    const deliveryStore$ = deliveryServices.getDeliveryList$();
    
    const [step, setStep] = useState<StepToDisplay>();

    useEffect(() => {
        return observe(() => {
            setStep(viewModel.getStep(stepId));
        });

    }, [deliveryStore$, stepId]);
    
    return {
        step
    }
}