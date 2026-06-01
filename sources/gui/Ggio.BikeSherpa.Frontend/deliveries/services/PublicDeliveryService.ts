import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {injectable} from "inversify";
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";

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
            deliveryType: 0
        }
    }
}