export default interface IPublicDeliveryService {
    loginPublicDeliveryCustomer: (email: string, code: string) => Promise<{name: string, deliveryType: number} | null>
}