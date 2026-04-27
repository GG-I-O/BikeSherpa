import {Step} from "@/steps/models/Step";
import JsonPatchDocument from "@/models/JsonPatchDocument";

export interface IDeliveryCustomBackendClientFacade {
    PatchStepEndpoint(step: Step, patch: JsonPatchDocument): Promise<void>
}