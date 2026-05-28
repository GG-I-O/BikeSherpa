import {Step} from "@/steps/models/Step";
import JsonPatchDocument from "@/models/JsonPatchDocument";
import Delivery from "@/deliveries/models/Delivery";
import UploadableFile from "@/models/UploadableFile";

export interface IDeliveryCustomBackendClientFacade {
    GetAllDailyDeliveriesEndpoint(date: string): Promise<Delivery[]>;
    PatchStepEndpoint(step: Step, patch: JsonPatchDocument): Promise<void>;
    PostStepCourierEndpoint(step: Step): Promise<void>;
    DeleteStepCourierEndpoint(step: Step): Promise<void>;
    PutStepOrderEndpoint(step: Step, increment: number): Promise<void>;
    PutStepTimeEndpoint(step: Step): Promise<void>;
    PutStepCompletionEndpoint(step: Step): Promise<void>;
    PostAttachmentEndpoint(step: Step, attachment: UploadableFile): Promise<void>;
}