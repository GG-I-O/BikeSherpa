import { IBackendClient } from "@/spi/BackendClientSPI";
import { injectable } from "inversify";
import Delivery, { DeliveryCrud } from "../models/Delivery";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import axios from "axios";
import DeliveryMapper from "./DeliveryMapper";
import { Link } from "@/models/HateoasLink";
import {Step} from "@/steps/models/Step";

@injectable()
export default class DeliveryBackendClientFacade implements IBackendClient<Delivery> {
    private apiClient;

    public constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    public async GetAllEndpoint(lastSync?: string): Promise<Delivery[]> {
        const data = await this.apiClient.GetAllDeliveriesEndpoint({
            params: { lastSync: lastSync ?? '' }
        });

        const deliveries = data.map((deliveryDto: { data: DeliveryCrud, links: Link[] | null }) => {
            return DeliveryMapper.DeliveryDtoToDelivery(deliveryDto);
        });

        return deliveries || [];
    }

    public async GetEndpoint(id: string): Promise<Delivery | null> {
        const response = await this.apiClient.GetDeliveryEndpoint({
            params: { deliveryId: id }
        });
        const delivery = DeliveryMapper.DeliveryDtoToDelivery(response);

        return delivery;
    }

    public async AddEndpoint(item: Delivery): Promise<string> {
        // Creating Delivery
        
        // Fill fields for Zod
        const deliveryData = {
            ...item,
            contractDate: new Date(item.contractDate).toISOString(),
            startDate: new Date(item.startDate).toISOString(),
            steps: []
        };
        const parsedDelivery = schemas.DeliveryCrud.safeParse(deliveryData);
        if (!parsedDelivery.success) {
            console.error("Parsing Create Delivery error:");
            console.error(parsedDelivery.error.format());
            throw parsedDelivery.error;
        }
        
        const deliveryResponse = await this.apiClient.AddDeliveryEndpoint(
            parsedDelivery.data,
            {
                headers: { operationId: item.operationId }
            }
        );
        
        // Creating steps associated
        for(let i = 0; i<item.steps.length; i++) {
            const step = {
                ...item.steps[i],
                estimatedDeliveryDate: new Date(item.steps[i].estimatedDeliveryDate).toISOString(),
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };
            const parsedStep = schemas.DeliveryStep.safeParse(step);
            if (!parsedStep.success) {
                console.error("Parsing Create Step error:");
                console.error(parsedStep.error.format());
                throw parsedStep.error;
            }

            await this.apiClient.AddDeliveryStepEndpoint(
                parsedStep.data,
                {
                    params: { deliveryId: deliveryResponse.id },
                    headers: { operationId: item.operationId }
                }
            );
        }

        return deliveryResponse.id;
    }

    public async UpdateEndpoint(item: Delivery): Promise<void> {
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

    public async DeleteEndpoint(item: Delivery): Promise<void> {
        await this.apiClient.DeleteDeliveryEndpoint(
            undefined,
            {
                params: { deliveryId: item.id },
                headers: { operationId: item.operationId }
            }
        );
    }
}