import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import StepDataTableRowViewModel from "@/steps/viewModel/StepDataTableRowViewModel";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {useState} from "react";
import {useDebounce} from "@/hooks/useDebounce";

export default function useStepDataTableRowViewModel(step: StepToDisplay) {
    const stepServices = IOCContainer.get<IStepServices>(StepServiceIdentifier.Services);
    const viewModel = new StepDataTableRowViewModel(stepServices);

    const splitTime = step.estimatedTime.split(':');
    
    const [comment, setComment] = useState(step.comment);
    useDebounce(() => {
        if (comment !== step.comment) {
            viewModel.updateComment(step.id, comment);
        }
    }, 1000, [comment]);
    
    return {
        updateStepTime: viewModel.updateStepTime,
        reorderStep: viewModel.reorderStep,
        updateStepTimeForADay: viewModel.updateStepTimeForADay,
        reorderStepForADay: viewModel.reorderStepForADay,
        updateComment: viewModel.updateComment,
        splitTime,
        comment,
        setComment
    }
}