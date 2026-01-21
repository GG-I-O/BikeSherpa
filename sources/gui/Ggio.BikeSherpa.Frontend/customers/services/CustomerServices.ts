import { Observable } from '@legendapp/state';
import * as Crypto from 'expo-crypto';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import Customer from '../models/Customer';
import InputCustomer from '../models/InputCustomer';
import { inject, injectable } from 'inversify';
import { IStorageContext } from '@/spi/StorageSPI';
import { ICustomerService } from '@/spi/CustomerSPI';
import { hateoasRel } from '@/models/HateoasLink';

@injectable()
export default class CustomerServices implements ICustomerService {
    private logger: ILogger;
    private storage: IStorageContext<Customer>;
    private readonly customerStore$: Observable<Record<string, Customer>>;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.CustomerStorage) customerStorage: IStorageContext<Customer>) {
        this.logger = logger;
        this.logger = this.logger.extend("Customer");
        this.storage = customerStorage;
        this.customerStore$ = this.storage.getStore();
    }

    /**
     * Subscribe to an Observable of customer list
     */
    public getCustomerList$(): Observable<Record<string, Customer>> {
        return this.customerStore$;
    }

    /**
     * Subscribe to an Observable of a single customer
     */
    public getCustomer$(customerId: string): Observable<Customer> {
        return this.customerStore$[customerId];
    }

    /**
     * Get a single customer without subscribing to changes
     */
    private getCustomer(customerId: string): Customer {
        return this.customerStore$[customerId].peek();
    }

    public deleteCustomer(customerId: string): void {
        const customer = this.getCustomer(customerId);
        const canDelete = customer.links?.some((link) => link.rel === hateoasRel.delete);

        if (!canDelete)
            throw new Error(`Cannot delete the customer ${customerId}`);
        this.customerStore$[customerId].delete();
    }

    // Wrapper for NewCustomerForm
    public createCustomer(customer: InputCustomer) {
        const newCustomer: Customer = {
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            ...customer,
        };
        this.customerStore$[newCustomer.id].set(newCustomer);
    };

    // Wrapper for EditCustomerForm
    public updateCustomer(customer: Customer) {
        const customerToUpdate = this.getCustomer(customer.id);
        const canUpdate = customerToUpdate.links?.some((link) => link.rel === hateoasRel.update);
        if (!canUpdate)
            throw new Error(`Cannot update customer ${customer.id}`);
        this.customerStore$[customer.id].assign(customer);
    };
}