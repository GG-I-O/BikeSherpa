import Customer, { CustomerCrud, CustomerDto } from "../models/Customer";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { createApiClient, schemas, getTagByAlias } from "@/infra/openAPI/client";
import axios from "axios";
import { Link } from "@/models/HateoasLink";
import CustomerMapper from "./CustomerMapper";

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

        const customers = data.map((customerDto: { data: CustomerCrud, links: Link[] | null }) => {
            return CustomerMapper.CustomerDtoToCustomer(customerDto);
        });

        return customers || [];
    }

    protected async getItem(id: string): Promise<Customer | null> {
        let customer: Customer | null;
        const link = this.getLinkHref(id, "self");
        if (link) {
            const response = await axios.get(link);
            const data = await response.data as CustomerDto;
            customer = CustomerMapper.CustomerDtoToCustomer(data);
        }
        else {
            const response = await this.apiClient.GetCustomerEndpoint({
                params: { customerId: id }
            });
            customer = CustomerMapper.CustomerDtoToCustomer(response);
        }
        return customer;
    }

    protected async create(item: Customer): Promise<string> {
        // Zod needs a complete body, even for optional fields
        item.siret = item.siret ?? null;
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

    protected async update(item: Customer): Promise<void> {
        // Zod needs a complete body, even for optional fields
        item.siret = item.siret ?? null;
        item.address.complement = item.address.complement ?? "";

        const parsed = schemas.CustomerCrud.safeParse(item);
        if (!parsed.success) {
            console.error("Create Debug (CustomerCrud validation failed):");
            console.error(parsed.error.format());
            throw parsed.error;
        }

        const customer = parsed.data;
        await this.apiClient.UpdateCustomerEndpoint(
            customer,
            {
                params: { customerId: customer.id },
                headers: { operationId: item.operationId }
            }
        );
    }

    protected async delete(item: Customer): Promise<void> {
        await this.apiClient.DeleteCustomerEndpoint(
            undefined,
            {
                params: { customerId: item.id },
                headers: { operationId: item.operationId }
            }
        );
    }
}