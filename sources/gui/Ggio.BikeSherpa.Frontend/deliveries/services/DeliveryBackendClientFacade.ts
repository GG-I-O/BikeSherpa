import { IBackendClient } from "@/spi/BackendClientSPI";
import { Delivery, DeliveryCrud } from "../models/Delivery";
import { injectable } from "inversify";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import axios from "axios";
import { Link } from "@/models/HateoasLink";
import DeliveryMapper from "./DeliveryMapper";

@injectable()
export class DeliveryBackendClientFacade implements IBackendClient<Delivery> {
    private apiClient;

    constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    async GetAllEndpoint(lastSync?: string): Promise<Delivery[]> {
        const data = await this.apiClient.GetAllDeliverysEndpoint({
            params: { lastSync: lastSync ?? '' }
        });

        const deliveries = data.map((deliveryDto: { data: DeliveryCrud, links: Link[] | null }) => {
            return DeliveryMapper.DeliveryDtoToDelivery(deliveryDto);
        });

        return deliveries || [];
    }

    async GetEndpoint(id: string): Promise<Delivery | null> {
        const response = await this.apiClient.GetDeliveryEndpoint({
            params: { deliveryId: id }
        });
        const delivery = DeliveryMapper.DeliveryDtoToDelivery(response);

        return delivery;
    }

    async AddEndpoint(item: Delivery): Promise<string> {
        const parsed = schemas.DeliveryCrud.safeParse(item);
        if (!parsed.success) {
            console.error("Create Debug (DeliveryCrud validation failed):");
            console.error(parsed.error.format());
            throw parsed.error;
        }

        const delivery = parsed.data;
        const response = await this.apiClient.AddDeliveryEndpoint(
            delivery,
            {
                headers: { operationId: item.operationId }
            }
        );

        // Synchronize with the item created at back-end
        return response.id;
    }

    async UpdateEndpoint(item: Delivery): Promise<void> {
        const parsed = schemas.DeliveryCrud.safeParse(item);
        if (!parsed.success) {
            console.error("Create Debug (DeliveryCrud validation failed):");
            console.error(parsed.error.format());
            throw parsed.error;
        }

        const delivery = parsed.data;
        await this.apiClient.UpdateDeliveryEndpoint(
            delivery,
            {
                params: { deliveryId: delivery.id },
                headers: { operationId: item.operationId }
            }
        );
    }

    async DeleteEndpoint(item: Delivery): Promise<void> {
        await this.apiClient.DeleteDeliveryEndpoint(
            undefined,
            {
                params: { deliveryId: item.id },
                headers: { operationId: item.operationId }
            }
        );
    }
}