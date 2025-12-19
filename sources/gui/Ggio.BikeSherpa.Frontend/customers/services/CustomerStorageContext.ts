import Customer from "../models/Customer";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { createApiClient, schemas, getTagByAlias } from "@/infra/openAPI/client";
import axios from "axios";
import { Link } from "@/models/HateoasLink";
import * as Crypto from 'expo-crypto';

@injectable()
export default class CustomerStorageContext extends AbstractStorageContext<Customer> {
    private apiClient;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService
    ) {
        const apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
        const tag = getTagByAlias("CreateCustomer") || "Customer";
        super(tag, logger, notificationService);
        this.apiClient = apiClient;
    }

    protected async getList(lastSync?: string): Promise<Customer[]> {
        const data = await this.apiClient.GetAllCustomersEndpoint({
            params: { lastSync: lastSync ?? '' }
        });

        const customers = data.map((customerDto: { data: Customer, links: Link[] | null }) => {
            let customer: Customer = { ...customerDto.data, links: customerDto.links ?? [] };
            return customer;
        });

        return customers || [];
    }

    protected async getItem(id: string): Promise<Customer | null> {
        let data;
        const link = this.getLinkHref(id, "self");
        if (link) {
            const response = await axios.get(link);
            data = await response.data;
        }
        else {
            const response = await this.apiClient.GetCustomerEndpoint({
                params: { customerId: id }
            });
            data = response.data;
        }
        return data;
    }

    protected async create(item: Customer): Promise<string> {
        const nowIso = new Date().toISOString();

        // Zod need a complete body, even for optional fields
        item.siret = item.siret ?? null;
        item.createdAt = nowIso;
        item.updatedAt = nowIso;
        item.address.complement = item.address.complement ?? "";

        const parsed = schemas.CustomerCrud.safeParse(item);
        if (!parsed.success) {
            console.error("Create Debug (CustomerCrud validation failed):");
            console.error(parsed.error.format());
            throw parsed.error;
        }

        const customer = parsed.data;
        const response = await this.apiClient.AddCustomerEndpoint(
            customer,
            {
                headers: { operationId: item.operationId }
            }
        );

        // Synchronize with the item created at back-end
        return response.id;
    }

    protected async update(item: Customer): Promise<Customer> {
        const link = this.getLinkHref(item.id, "update");
        if (!link)
            throw new Error(`Cannot update the customer ${item.id}`);

        const response = await axios.put(
            link,
            JSON.stringify(item),
            {
                headers: { operationId: item.operationId }
            }
        );
        if (response.status !== 200)
            throw new Error(`Could not update the customer ${item.id}`);

        return item;
    }

    protected async delete(item: Customer): Promise<void> {
        const link = this.getLinkHref(item.id, "delete");
        if (!link)
            throw new Error(`Cannot delete the customer ${item.id}`);

        const response = await axios.delete(
            link,
            {
                headers: { operationId: item.operationId }
            }
        );
        if (response.status !== 200)
            throw new Error(`Could not delete the customer ${item.id}`);
    }
}