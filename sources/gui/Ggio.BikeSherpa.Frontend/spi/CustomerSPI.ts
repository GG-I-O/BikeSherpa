import Customer from "@/customers/models/Customer";
import InputCustomer from "@/customers/models/InputCustomer";
import { Observable } from "@legendapp/state";

export interface ICustomerService {
    getCustomerList$(): Observable<Record<string, Customer>>;
    getCustomer$(customerId: string): Observable<Customer>;
    createCustomer(customer: InputCustomer): void;
    updateCustomer(customer: Customer): void;
    deleteCustomer(customerId: string): void;
}