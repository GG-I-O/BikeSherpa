import CustomerStorageContext from '../services/CustomerStorageContext';
import { Observable } from '@legendapp/state';
import * as Crypto from 'expo-crypto';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import Customer from '../models/Customer';
import InputCustomer from '../models/InputCustomer';

class CustomerViewModel {
    private static instance: CustomerViewModel;
    private logger: ILogger;
    private storageContext: CustomerStorageContext;
    private readonly customerStore$: Observable<Record<string, Customer>>;

    private constructor() {
        this.logger = IOCContainer.get<ILogger>(ServicesIdentifiers.Logger);
        this.logger = this.logger.extend("Customer");
        this.storageContext = IOCContainer.get(ServicesIdentifiers.CustomerStorage);
        this.customerStore$ = this.storageContext.getStore();
    }

    public static getInstance(): CustomerViewModel {
        if (!CustomerViewModel.instance) {
            CustomerViewModel.instance = new CustomerViewModel();
        }
        return CustomerViewModel.instance;
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

export default function useCustomerViewModel() {
    return CustomerViewModel.getInstance();
}