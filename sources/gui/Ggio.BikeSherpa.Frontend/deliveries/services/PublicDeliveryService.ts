import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {injectable} from "inversify";
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";
import Delivery from "@/deliveries/models/Delivery";
import Customer from "@/customers/models/Customer";

@injectable()
export default class PublicDeliveryService implements IPublicDeliveryService {
    private apiClient;

    public constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    public loginPublicDeliveryCustomer = async (email: string, code: string) => {
        // const result = await this.apiClient.loginPublicDeliveryCustomer({params: {email: email, code: code} });
        
        return {
            name: "test",
            deliveryType: 1
        }
    }
    
    public createDelivery = async (delivery: Delivery, customer: Customer): Promise<void> => {
        console.debug("createDelivery")
        console.debug(delivery)
        console.debug(customer)
        // const result = await this.apiClient.createPublicDelivery({params: {delivery: delivery, customer: customer} });
    }
}