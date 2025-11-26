import * as zod from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import CustomerStorageContext from '../services/CustomerStorageContext';
import { Observable } from '@legendapp/state';
import * as Crypto from 'expo-crypto';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ServicesIndentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import Customer from '../models/Customer';
import InputCustomer from '../models/InputCustomer';



class CustomerViewModel {
    private static instance: CustomerViewModel;
    private logger: ILogger;
    private storageContext: CustomerStorageContext;
    private customerStore$: Observable<Record<string, Customer>>;

    private constructor() {
        // this.logger = IOCContainer.get(ServicesIndentifiers.Logger);
        // this.logger = this.logger.extend("Customer");
        this.logger = {
            info: (...args: unknown[]) => console.info(args),
            error: (...args: unknown[]) => console.error(args),
            warn: (...args: unknown[]) => console.warn(args),
            debug: (...args: unknown[]) => console.debug(args),
            extend: (extension: string) => this.logger
        }
        this.storageContext = IOCContainer.get(ServicesIndentifiers.CustomerStorage);
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
    private updateCustomer(customer: Customer) {
        try {
            this.customerStore$[customer.id].assign(customer);
        } catch (e) {
            this.logger.error("updateCustomer Error :", e);
        }
    };

    /**
     * @returns react-hook-form for updating an existing customer
     */
    // public getEditCustomerForm() {
    //     const {
    //         control,
    //         handleSubmit,
    //         formState: { errors },
    //         setValue
    //     } = useForm<Customer>({
    //         defaultValues: {
    //             id: '',
    //             firstName: '',
    //             lastName: '',
    //             phoneNumber: ''
    //         },
    //         resolver: zodResolver(editCustomerSchema)
    //     });

    //     const onSubmit = (customer: Customer) => {
    //         this.updateCustomer(customer);
    //     };

    //     return { control, handleSubmit: handleSubmit(onSubmit), errors, setValue }
    // }
}

export default function useCustomerViewModel() {
    return CustomerViewModel.getInstance();
}