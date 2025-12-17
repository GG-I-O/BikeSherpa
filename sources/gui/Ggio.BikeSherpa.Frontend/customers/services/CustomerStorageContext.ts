import Customer from "../models/Customer";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import axios from "axios";
import { Link } from "@/models/HateoasLink";

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
        const link = this.getLinkHref(id, "self");
        if (!link)
            throw new Error(`Cannot read the customer with id ${id}`);

        const response = await axios.get(link);
        const data = await response.data;
        return data;
    }

    protected async create(item: Customer): Promise<Customer> {
        const customer = schemas.CustomerCrud.parse(item);
        const response = await this.apiClient.AddCustomerEndpoint(customer);

        // Synchronize with the item created at back-end
        return await this.getItem(response.id) ?? item;
    }

    protected async update(item: Customer): Promise<Customer> {
        const link = this.getLinkHref(item.id, "update");
        if (!link)
            throw new Error(`Cannot update the customer ${item.id}`);

        const response = await axios.put(link, JSON.stringify(item));
        if (response.status !== 200)
            throw new Error(`Could not update the customer ${item.id}`);

        return item;
    }

    protected async delete(item: Customer): Promise<void> {
        const link = this.getLinkHref(item.id, "delete");
        if (!link)
            throw new Error(`Cannot delete the customer ${item.id}`);

        const response = await axios.delete(link);
        if (response.status !== 200)
            throw new Error(`Could not delete the customer ${item.id}`);
    }
}