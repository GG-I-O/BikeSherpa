import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import UnassignedStepDetailViewModel from "@/steps/viewModel/UnassignedStepDetailViewModel";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {IUserService} from "@/spi/AuthSPI";
import {useEffect, useState} from "react";

export default function useUnassignedStepDetailViewModel() {
    const stepServices = IOCContainer.get<IStepServices>(StepServiceIdentifier.Services);
    const userService = IOCContainer.get<IUserService>(ServicesIdentifiers.UserService);

    const viewModel = new UnassignedStepDetailViewModel(stepServices);
    
    const [currentCourierId, setCurrentCourierId] = useState<string>("");

    useEffect(() => {
        const currentUserId = userService.getUserLogInfo()?.id;
        if (!currentUserId) return;
        setCurrentCourierId(currentUserId);
    }, [userService, setCurrentCourierId]);

    return {
        assignMyself: (stepId: string) => viewModel.assignCourier(stepId, currentCourierId)
    };
}