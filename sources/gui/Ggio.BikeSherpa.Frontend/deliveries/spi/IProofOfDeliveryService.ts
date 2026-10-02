export interface IProofOfDeliveryService {
    exportProofOfDelivery(deliveryId: string): Promise<string>;
}