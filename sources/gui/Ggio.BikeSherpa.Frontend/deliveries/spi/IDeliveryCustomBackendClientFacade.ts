import {Step} from "@/steps/models/Step";
import JsonPatchDocument from "@/models/JsonPatchDocument";
import Delivery from "@/deliveries/models/Delivery";
import UploadableFile from "@/models/UploadableFile";

export interface IDeliveryCustomBackendClientFacade {
    PutWaitingDeliveryEndpoint(delivery: Delivery): Promise<void>;
    PutValidateDeliveryEndpoint(delivery: Delivery): Promise<void>;
    
    GetAllMyDeliveriesEndpoint(date: string): Promise<Delivery[]>;
    GetAllUnassignedDeliveriesEndpoint(date: string): Promise<Delivery[]>;
    PatchStepEndpoint(step: Step, patch: JsonPatchDocument): Promise<void>;
    PostStepCourierEndpoint(step: Step): Promise<void>;
    DeleteStepCourierEndpoint(step: Step): Promise<void>;
    PutStepAssignMyself(step: Step): Promise<void>;
    PutStepOrderEndpoint(step: Step, increment: number): Promise<void>;
    PutStepTimeEndpoint(step: Step): Promise<void>;
    PutStepCompletionEndpoint(step: Step): Promise<void>;
    PostAttachmentEndpoint(step: Step, attachment: UploadableFile): Promise<void>;
    PutSignatureEndpoint(step: Step, attachment: UploadableFile, receiver: string): Promise<void>;
}