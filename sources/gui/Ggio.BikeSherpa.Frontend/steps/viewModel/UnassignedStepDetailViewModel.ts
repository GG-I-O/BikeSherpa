import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import {inject} from "inversify";

export default class UnassignedStepDetailViewModel {
    private readonly stepServices: IStepServices;

    constructor(
        @inject(StepServiceIdentifier.Services) stepServices: IStepServices
    ) {
        this.stepServices = stepServices;
    }

    public assignMyself = (stepId: string) => {
        this.stepServices.assignMyself(stepId);
    }
}