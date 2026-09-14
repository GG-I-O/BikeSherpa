import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import UnassignedStepDetailViewModel from "@/steps/viewModel/UnassignedStepDetailViewModel";

export default function useUnassignedStepDetailViewModel() {
    const stepServices = IOCContainer.get<IStepServices>(StepServiceIdentifier.Services);

    const viewModel = new UnassignedStepDetailViewModel(stepServices);

    return {
        assignMyself: viewModel.assignMyself
    };
}