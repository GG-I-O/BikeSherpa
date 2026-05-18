import Delivery from "@/deliveries/models/Delivery";

export interface IDeliveryStorageMiddleware {
    setGetAllDateForDailyDeliveries(date: string | null): void;
    getAll(date?: string): Promise<Delivery[]>;
    addUpdateStepState(deliveryId: string, stepId: string, state: string): void;
    update(delivery: Delivery): Promise<void>;
}