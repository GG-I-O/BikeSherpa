import { ICustomerService } from "@/spi/CustomerSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import Customer from "../models/Customer";
import {customerFormBaseSchema, CustomerFormValues} from "./zod/customerFormBaseSchema";
import CustomerMapper from "@/customers/services/CustomerMapper";

export default class NewCustomerFormViewModel {
    private customerServices: ICustomerService;
    private resetCallback?: UseFormReset<CustomerFormValues>;

    constructor(
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.customerServices = customerServices;
    }

    public onSubmit = (customer: CustomerFormValues): void => {
        const inputCustomer = CustomerMapper.CustomerFormValuesToInputCustomer(customer);
        customer.address.name = customer.name;
        this.customerServices.createCustomer(inputCustomer);
        
        if (this.resetCallback) {
            this.resetCallback(); // Clear form after submission
        }
    };

    public setResetCallback(reset?: UseFormReset<CustomerFormValues>) {
        this.resetCallback = reset;
    }

    public getNewCustomerSchema(customerList: Customer[]) {
        return customerFormBaseSchema.extend({
            code: customerFormBaseSchema.shape.code.refine(
                (value) => !customerList.some((customer) => customer.code === value),
                "Le code doit être unique"
            ),
        })
    }
}