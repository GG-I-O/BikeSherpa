import Customer from "@/customers/models/Customer";
import Delivery from "@/deliveries/models/Delivery";
import PublicDeliveryEstimatedValue from "@/deliveries/models/PublicDeliveryEstimatedValue";

export default interface IPublicDeliveryService {
    loginPublicDeliveryCustomer: (email: string, code: string) => Promise<{name: string, deliveryType: number} | null>;
    
    getEstimatedValue: (delivery: Delivery) => Promise<PublicDeliveryEstimatedValue>;
    
    getVatRate: () => Promise<number>;
    
    getLastHourToOrder: () => Promise<number>;
    getUrgenciesLastHourToOrder: () => Promise<{value: string, label: string, lastHourToOrder: number}[]>;
    getWorkHours: () => Promise<{startDate: string, endDate: string}>;
    
    createDelivery: (delivery: Delivery, customer: Customer) => Promise<boolean>;
}