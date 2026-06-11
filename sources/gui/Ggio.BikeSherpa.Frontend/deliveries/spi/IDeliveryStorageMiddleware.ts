import Delivery from "@/deliveries/models/Delivery";
import UploadableFile from "@/models/UploadableFile";

export interface IDeliveryStorageMiddleware {
    setGetAllDateForDailyDeliveries(date: string | null): void;
    getAll(date?: string): Promise<Delivery[]>;
    
    addUpdateDeliveryState(deliveryId: string, state: string): void;
    addUpdateStepState(deliveryId: string, stepId: string, state: string): void;
    addAttachmentToUploadQueue(stepId: string, file: UploadableFile): void;
    update(delivery: Delivery): Promise<void>;
}