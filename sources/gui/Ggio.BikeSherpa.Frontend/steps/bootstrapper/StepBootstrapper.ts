import {Container} from "inversify";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import StepServices from "@/steps/services/StepServices";
import StepMapper from "@/steps/services/StepMapper";
import IStepMapper from "@/steps/spi/IStepMapper";
import {UnassignedBadgeService} from "@/steps/services/UnassignedBadgeService";
import {IUnassignedBadgeService} from "@/steps/spi/IUnassignedBadgeService";

export default class StepBootstrapper {
    public static init(IOCContainer: Container) {
        IOCContainer.bind<IStepServices>(StepServiceIdentifier.Services).to(StepServices).inSingletonScope();
        
        IOCContainer.bind<IStepMapper>(StepServiceIdentifier.Mapper).to(StepMapper).inSingletonScope();
        
        IOCContainer.bind<IUnassignedBadgeService>(StepServiceIdentifier.UnassignedBadgeService).to(UnassignedBadgeService).inSingletonScope();
    }
}