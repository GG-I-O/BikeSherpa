import Delivery from "@/deliveries/models/Delivery";

export interface IDeliveryStorageMiddleware {
    setGetAllDateForDailySteps(date: string | null): void;
    getAll(date?: string): Promise<Delivery[]>;
    addUpdateStepState(deliveryId: string, stepId: string, state: string): void;
    update(delivery: Delivery): Promise<void>;
}