import Customer from "../models/Customer";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import axios from "axios";

@injectable()
export default class CustomerStorageContext extends AbstractStorageContext<Customer> {
    private apiClient;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService
    ) {
        super("Customers", logger, notificationService);
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    protected async getList(lastSync?: string): Promise<Customer[]> {
        try {
            const data = await this.apiClient.GetAllCustomersEndpoint({
                params: { lastSync: lastSync ?? '' }
            });

            const customers = data.map((customerDto) => {
                let customer: Customer = {...customerDto.data, links: customerDto.links ?? []};
                return customer;
            });

            return customers || [];
        } catch (e) {
            this.logger.error('GetAll error:', e);
            return [];
        }
    }
    protected async getItem(id: string): Promise<Customer | null> {
        try {
            const link = this.getLinkHref(id, "self");
            if (!link)
                throw new Error(`Cannot read the customer with id ${id}`);

            const response = await axios.get(link);
            const data = await response.data;
            return data;
        } catch (e) {
            this.logger.error('Get error:', e);
            return null;
        }
    }
    protected async create(item: Customer): Promise<Customer> {
        try {
            const customer = schemas.CustomerCrud.parse(item);

            const response = await this.apiClient.AddCustomerEndpoint(customer);
            
            // Synchronize with the item created at back-end
            return await this.getItem(response.id) ?? item;
        } catch (e) {
            this.logger.error('Post error:', e);
            throw e; // Throw to legendapp
        }
    }
    protected async update(item: Customer): Promise<Customer> {
        try {
            const link = this.getLinkHref(item.id, "update");
            if (!link)
                throw new Error(`Cannot update the customer ${item.id}`);

            const response = await axios.put(link, JSON.stringify(item));
            if (response.status !== 200)
                throw new Error(`Could not update the customer ${item.id}`);

            return item;
        } catch (e) {
            this.logger.error('Put error:', e);
            throw e; // Throw to legendapp
        }
    }
    protected async delete(item: Customer): Promise<void> {
        try {
            const link = this.getLinkHref(item.id, "delete");
            if (!link)
                throw new Error(`Cannot delete the customer ${item.id}`);

            const response = await axios.delete(link);
            if (response.status !== 200)
                throw new Error(`Could not delete the customer ${item.id}`);

        } catch (e) {
            this.logger.error("Delete error:", e);
            throw e;
        }
    }
}