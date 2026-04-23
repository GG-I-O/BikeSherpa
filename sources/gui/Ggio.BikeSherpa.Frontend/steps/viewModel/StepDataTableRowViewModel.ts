import {inject} from "inversify";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";

export default class StepDataTableRowViewModel { 
    private stepServices: IStepServices;
    
    constructor(
        @inject(StepServiceIdentifier.Services) stepServices: IStepServices
    ) {
        this.stepServices = stepServices;
    }
    
    public updateStepTime = (stepId: string, hours: number, minutes: number): void => {
        this.stepServices.updateTime(stepId, hours, minutes);
    }
}