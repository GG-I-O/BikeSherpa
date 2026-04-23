import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import StepDataTableRowViewModel from "@/steps/viewModel/StepDataTableRowViewModel";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";

export default function useStepDataTableRowViewModel() {
    const stepServices = IOCContainer.get<IStepServices>(StepServiceIdentifier.Services);
    
    const viewModel = new StepDataTableRowViewModel(stepServices);
    
    return {
        updateStepTime: viewModel.updateStepTime
    }
}