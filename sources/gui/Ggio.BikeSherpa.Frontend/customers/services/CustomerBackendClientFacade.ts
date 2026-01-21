import { IBackendClient } from "@/spi/BackendClientSPI";
import Customer, { CustomerCrud } from "../models/Customer";
import { injectable } from "inversify";
import { createApiClient, schemas } from "@/infra/openAPI/client";
import axios from "axios";
import { Link } from "@/models/HateoasLink";
import CustomerMapper from "./CustomerMapper";

@injectable()
export class CustomerBackendClientFacade implements IBackendClient<Customer> {
    private apiClient;

    constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    async GetAllEndpoint(lastSync?: string): Promise<Customer[]> {
        const data = await this.apiClient.GetAllCustomersEndpoint({
            params: { lastSync: lastSync ?? '' }
        });

        const customers = data.map((customerDto: { data: CustomerCrud, links: Link[] | null }) => {
            return CustomerMapper.CustomerDtoToCustomer(customerDto);
        });

        return customers || [];
    }

    async GetEndpoint(id: string): Promise<Customer | null> {
        const response = await this.apiClient.GetCustomerEndpoint({
            params: { customerId: id }
        });
        const customer = CustomerMapper.CustomerDtoToCustomer(response);

        return customer;
    }

    async AddEndpoint(item: Customer): Promise<string> {
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

    async UpdateEndpoint(item: Customer): Promise<void> {
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

    async DeleteEndpoint(item: Customer): Promise<void> {
        await this.apiClient.DeleteCustomerEndpoint(
            undefined,
            {
                params: { customerId: item.id },
                headers: { operationId: item.operationId }
            }
        );
    }

}