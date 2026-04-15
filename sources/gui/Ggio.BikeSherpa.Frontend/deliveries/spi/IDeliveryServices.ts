import { Observable } from "@legendapp/state";
import Delivery from "../models/Delivery";
import InputDelivery from "../models/InputDelivery";

export interface IDeliveryServices {
    getDeliveryList$(): Observable<Record<string, Delivery>>;
    getDelivery$(deliveryId: string): Observable<Delivery>;
    createDelivery(delivery: Delivery): void;
    updateDelivery(delivery: Delivery): void;
    deleteDelivery(deliveryId: string): void;
}