import {IProofOfDeliveryService} from "@/deliveries/spi/IProofOfDeliveryService";
import {injectable} from "inversify";
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";

@injectable()
export default class ProofOfDeliveryService implements IProofOfDeliveryService {
    private readonly apiClient;

    public constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }
    
    public async exportProofOfDelivery(deliveryId: string): Promise<string> {
        return await this.apiClient.ExportProofOfDelivery({
            params: {
                deliveryId: deliveryId
            }
        });
    }
}