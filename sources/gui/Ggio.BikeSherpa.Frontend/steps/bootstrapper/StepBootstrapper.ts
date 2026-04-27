import {Container} from "inversify";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import StepServices from "@/steps/services/StepServices";

export default class StepBootstrapper {
    public static init(IOCContainer: Container) {
        IOCContainer.bind<IStepServices>(StepServiceIdentifier.Services).to(StepServices).inSingletonScope();
    }
}