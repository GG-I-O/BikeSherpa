import { IBackendClient } from "@/spi/BackendClientSPI";
import Courier, { CourierCrud } from "../models/Courier";
import { injectable } from "inversify";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import axios from "axios";
import { Link } from "@/models/HateoasLink";
import CourierMapper from "./CourierMapper";

@injectable()
export class CourierBackendClientFacade implements IBackendClient<Courier> {
    private apiClient;

    constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    async GetAllEndpoint(lastSync?: string): Promise<Courier[]> {
        const data = await this.apiClient.GetAllCouriersEndpoint({
            params: { lastSync: lastSync ?? '' }
        });

        const couriers = data.map((courierDto: { data: CourierCrud, links: Link[] | null }) => {
            return CourierMapper.CourierDtoToCourier(courierDto);
        });

        return couriers || [];
    }

    async GetEndpoint(id: string): Promise<Courier | null> {
        const response = await this.apiClient.GetCourierEndpoint({
            params: { courierId: id }
        });
        const courier = CourierMapper.CourierDtoToCourier(response);

        return courier;
    }

    async AddEndpoint(item: Courier): Promise<string> {
        // Zod needs a complete body, even for optional fields
        item.address.complement = item.address.complement ?? "";
        const parsed = schemas.CourierCrud.safeParse(item);
        if (!parsed.success) {
            console.error("Create Debug (CourierCrud validation failed):");
            console.error(parsed.error.format());
            throw parsed.error;
        }

        const courier = parsed.data;
        const response = await this.apiClient.AddCourierEndpoint(
            courier,
            {
                headers: { operationId: item.operationId }
            }
        );

        // Synchronize with the item created at back-end
        return response.id;
    }

    async UpdateEndpoint(item: Courier): Promise<void> {
        // Zod needs a complete body, even for optional fields
        item.address.complement = item.address.complement ?? "";
        const parsed = schemas.CourierCrud.safeParse(item);
        if (!parsed.success) {
            console.error("Create Debug (CourierCrud validation failed):");
            console.error(parsed.error.format());
            throw parsed.error;
        }

        const courier = parsed.data;
        await this.apiClient.UpdateCourierEndpoint(
            courier,
            {
                params: { courierId: courier.id },
                headers: { operationId: item.operationId }
            }
        );
    }

    async DeleteEndpoint(item: Courier): Promise<void> {
        await this.apiClient.DeleteCourierEndpoint(
            undefined,
            {
                params: { courierId: item.id },
                headers: { operationId: item.operationId }
            }
        );
    }
}