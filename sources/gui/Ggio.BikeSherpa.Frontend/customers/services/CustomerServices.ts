import { Observable } from '@legendapp/state';
import * as Crypto from 'expo-crypto';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import Customer from '../models/Customer';
import InputCustomer from '../models/InputCustomer';
import { inject, injectable } from 'inversify';
import { IStorageContext } from '@/spi/StorageSPI';
import { ICustomerService } from '@/spi/CustomerSPI';

@injectable()
export default class CustomerServices implements ICustomerService {
    private logger: ILogger;
    private storageContext: IStorageContext<Customer>;
    private readonly customerStore$: Observable<Record<string, Customer>>;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.CustomerStorage) customerContext: IStorageContext<Customer>) {
        this.logger = logger;
        this.logger = this.logger.extend("Customer");
        this.storageContext = customerContext;
        this.customerStore$ = this.storageContext.getStore();
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
    public getCustomer(customerId: string): Customer {
        return this.customerStore$[customerId].peek();
    }

    public deleteCustomer(customerId: string): void {
        this.customerStore$[customerId].delete();
    }

    // Wrapper for NewCustomerForm
    public createCustomer(customer: InputCustomer) {
        try {
            const newCustomer: Customer = {
                id: Crypto.randomUUID(),
                operationId: Crypto.randomUUID(),
                ...customer,
            };
            this.customerStore$[newCustomer.id].set(newCustomer);
        } catch (e) {
            this.logger.error("createCustomer Error :", e);
        }
    };

    // Wrapper for EditCustomerForm
    public updateCustomer(customer: Customer) {
        try {
            this.customerStore$[customer.id].assign(customer);
        } catch (e) {
            this.logger.error("updateCustomer Error :", e);
        }
    };
}