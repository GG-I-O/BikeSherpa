import {Step} from "@/steps/models/Step";
import JsonPatchDocument from "@/models/JsonPatchDocument";

export interface IDeliveryCustomBackendClientFacade {
    PatchStepEndpoint(step: Step, patch: JsonPatchDocument): Promise<void>;
    PostStepCourierEndpoint(step: Step): Promise<void>;
    DeleteStepCourierEndpoint(step: Step): Promise<void>;
    PutStepOrderEndpoint(step: Step, increment: number): Promise<void>;
}