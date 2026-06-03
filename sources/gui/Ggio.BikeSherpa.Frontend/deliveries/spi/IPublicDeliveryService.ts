import Customer from "@/customers/models/Customer";
import Delivery from "@/deliveries/models/Delivery";

export default interface IPublicDeliveryService {
    loginPublicDeliveryCustomer: (email: string, code: string) => Promise<{name: string, deliveryType: number} | null>;
    
    createDelivery: (delivery: Delivery, customer: Customer) => Promise<boolean>
}