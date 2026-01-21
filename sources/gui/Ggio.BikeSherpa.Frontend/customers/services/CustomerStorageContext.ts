import Customer, { CustomerDto } from "../models/Customer";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { getTagByAlias } from "@/infra/openAPI/client";
import axios from "axios";
import CustomerMapper from "./CustomerMapper";
import { IBackendClient } from "@/spi/BackendClientSPI";

@injectable()
export default class CustomerStorageContext extends AbstractStorageContext<Customer> {
    private backendClient;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService,
        @inject(ServicesIdentifiers.CustomerBackendClientFacade) customerBackendClientFacade: IBackendClient<Customer>
    ) {
        const tag = getTagByAlias("CreateCustomer") || "Customer";
        super(tag, logger, notificationService);
        this.backendClient = customerBackendClientFacade;
    }

    protected async getList(lastSync?: string): Promise<Customer[]> {
        return await this.backendClient.GetAllEndpoint(lastSync);
    }

    protected async getItem(id: string): Promise<Customer | null> {
        let customer: Customer | null;
        const link = this.getLinkHref(id, "self");
        if (link) {
            const response = await axios.get(link);
            const data = await response.data as CustomerDto;
            customer = CustomerMapper.CustomerDtoToCustomer(data);
            return customer;
        }
        else {
            return await this.backendClient.GetEndpoint(id);
        }
    }

    protected async create(item: Customer): Promise<string> {
        return await this.backendClient.AddEndpoint(item);
    }

    protected async update(item: Customer): Promise<void> {
        return await this.backendClient.UpdateEndpoint(item);
    }

    protected async delete(item: Customer): Promise<void> {
        return await this.backendClient.DeleteEndpoint(item);
    }
}