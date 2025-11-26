
import Customer from "../models/Customer";
import axios from "axios";
import { inject, injectable } from "inversify";
import { ServicesIndentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import { INotificationService } from "@/spi/StorageSPI";
import { Link } from "@/models/HateoasLink";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
// import { createApiClient } from "@/services/OpenAPI/client";

@injectable()
export default class CustomerStorageContext extends AbstractStorageContext<Customer> {
    // private apiClient;

    public constructor(
        @inject(ServicesIndentifiers.Logger) logger: ILogger,
        // @inject(ServicesIndentifiers.NotificationService) notificationService: INotificationService
    ) {
        // super("Customers", logger, notificationService);
        super("Customers", logger);

        // this.apiClient = createApiClient(axios.defaults.baseURL || '', {
        //     axiosInstance: axios
        // });
    }

    protected async getList(lastSync?: string): Promise<Customer[]> {
        try {
            return [];

            // const data = await this.apiClient.GetCustomers({
            //     queries: { lastSync }
            // });
            // return data || [];
        } catch (e) {
            this.logger.error('GetAll error:', e);
            return [];
        }
    }
    protected async getItem(id: string): Promise<Customer | null> {
        try {
            const store = this.getStore();
            return store[id].peek();
            // return await this.apiClient.GetCustomer({
            //     params: { id: id }
            // });
        } catch (e) {
            this.logger.error('Get error:', e);
            return null;
        }
    }
    protected async create(item: Customer): Promise<Customer> {
        try {
            return item;

            // return await this.apiClient.CreateCustomer(item);
        } catch (e) {
            this.logger.error('Post error:', e);
            throw e; // Throw to legendapp
        }
    }
    protected async update(item: Customer): Promise<Customer> {
        try {
            return item;
            // const response = await axios.put(
            //     item.links?.find((link: Link) => link.rel == "update")?.href ?? '',
            //     JSON.stringify(item)
            // );

            // await this.apiClient.UpdateCustomer(item, { params: { id: item.id } });
            return item;
        } catch (e) {
            this.logger.error('Put error:', e);
            throw e; // Throw to legendapp
        }
    }
    protected async delete(item: Customer): Promise<void> {
        try {
            if (!item.id)
                throw new Error("Customer need an ID to delete");
            // const response = await axios.delete(
            //     item.links?.find((link: Link) => link.rel == "delete")?.href ?? ''
            // );

            // await this.apiClient.DeleteCustomer(undefined, { params: { id: item.id } })
        } catch (e) {
            this.logger.error("Delete error:", e);
            throw e;
        }
    }
}