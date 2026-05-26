import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import Delivery from "@/deliveries/models/Delivery";
import {Step} from "@/steps/models/Step";
import IStepMapper from "@/steps/spi/IStepMapper";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import {IStepServices} from "@/steps/spi/IStepServices";
import UploadableFile from "@/models/UploadableFile";

export default class StepDetailViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly stepServices: IStepServices;
    private readonly stepMapper: IStepMapper;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(StepServiceIdentifier.Services) stepServices: IStepServices,
        @inject(StepServiceIdentifier.Mapper) stepMapper: IStepMapper
    ) {
        this.deliveryServices = deliveryServices;
        this.stepServices = stepServices;
        this.stepMapper = stepMapper;
    }

    public getStep = (stepId: string): StepToDisplay | undefined => {
        if (!this.deliveryServices)
            return undefined;

        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());
        let delivery: Delivery | undefined;
        let step: Step | undefined;
        deliveries.forEach(d => {
            const s = d.steps.find((step) => step.id === stepId);
            if (s) {
                delivery = d;
                step = s;
                return;
            }
        });
        if (!delivery || !step)
            return undefined;

        return this.stepMapper.StepToStepToDisplay(delivery, step);
    }

    public updateCourierComment = (stepId: string, comment: string) => {
        this.stepServices.updateCourierComment(stepId, comment);
    }

    public stepComplete = (stepId: string, complete: boolean) => {
        this.stepServices.completeStep(stepId, complete);
    }

    public addAttachment = (stepId: string, file: UploadableFile) => {
        this.stepServices.addAttachment(stepId, file);
    }
}