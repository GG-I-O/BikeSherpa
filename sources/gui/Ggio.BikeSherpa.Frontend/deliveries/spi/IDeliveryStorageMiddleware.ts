import Delivery from "@/deliveries/models/Delivery";

export interface IDeliveryStorageMiddleware {
    addUpdateStepState(deliveryId: string, stepId: string, state: string): void;
    update(delivery: Delivery): Promise<void>;
}