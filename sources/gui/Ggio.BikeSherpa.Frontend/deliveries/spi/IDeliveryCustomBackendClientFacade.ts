import {Step} from "@/steps/models/Step";

export interface IDeliveryCustomBackendClientFacade {
    PatchStepTimeEndpoint(step: Step): Promise<void>
    PatchStepOrderEndpoint(step: Step): Promise<void>
}