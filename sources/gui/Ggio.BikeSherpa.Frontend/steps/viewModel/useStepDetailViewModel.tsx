import {useEffect, useState} from "react";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import StepDetailViewModel from "@/steps/viewModel/StepDetailViewModel";
import {observe} from "@legendapp/state";
import IStepMapper from "@/steps/spi/IStepMapper";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";

export default function useStepDetailViewModel(stepId: string) {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const stepMapper = IOCContainer.get<IStepMapper>(StepServiceIdentifier.Mapper);
    const viewModel = new StepDetailViewModel(deliveryServices, stepMapper);

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