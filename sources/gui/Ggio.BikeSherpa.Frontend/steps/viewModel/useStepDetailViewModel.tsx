import {useEffect, useState} from "react";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import StepDetailViewModel from "@/steps/viewModel/StepDetailViewModel";
import {observe} from "@legendapp/state";
import IStepMapper from "@/steps/spi/IStepMapper";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import {IStepServices} from "@/steps/spi/IStepServices";
import {useDebounce} from "@/hooks/useDebounce";

export default function useStepDetailViewModel(stepId: string) {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const stepServices = IOCContainer.get<IStepServices>(StepServiceIdentifier.Services);
    const stepMapper = IOCContainer.get<IStepMapper>(StepServiceIdentifier.Mapper);
    const viewModel = new StepDetailViewModel(deliveryServices, stepServices, stepMapper);

    const deliveryStore$ = deliveryServices.getDeliveryList$();
    
    const [step, setStep] = useState<StepToDisplay>();

    useEffect(() => {
        return observe(() => {
            setStep(viewModel.getStep(stepId));
        });

    }, [deliveryStore$, stepId]);

    const [courierComment, setCourierComment] = useState<string>('');
    useEffect(() => {
        if (step) {
            setCourierComment(step.courierComment);
        }
    }, [step])
    
    useDebounce(() => {
        if (step && courierComment !== step.courierComment) {
            viewModel.updateCourierComment(step.id, courierComment);
        }
    }, 1000, [courierComment]);
    
    return {
        step,
        courierComment,
        setCourierComment,
        completeStep: () => viewModel.stepComplete(stepId, true),
        cancelStep: () => viewModel.stepComplete(stepId, false)
    }
}