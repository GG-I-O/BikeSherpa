import { Delivery } from "@/deliveries/models/Delivery";
import { InputDelivery } from "@/deliveries/models/InputDelivery";
import { Observable } from "@legendapp/state";

export interface IDeliveryService {
    getDeliveryList$(): Observable<Record<string, Delivery>>;
    getDelivery$(deliveryId: string): Observable<Delivery>;
    createDelivery(delivery: InputDelivery): void;
    updateDelivery(delivery: Delivery): void;
    deleteDelivery(deliveryId: string): void;
}