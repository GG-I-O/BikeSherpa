import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICustomerService} from "@/spi/CustomerSPI";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {inject} from "inversify";
import {deliveryFormBaseSchema} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import Customer from "@/customers/models/Customer";

export default abstract class AbstractFormViewModel {
    protected deliveryServices: IDeliveryServices;
    protected customerServices: ICustomerService;

    public constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService
    ) {
        this.deliveryServices = deliveryServices;
        this.customerServices = customerServices;
    }

    // Add Customer validation
    public getDeliverySchema = () => {
        const customerList = Object.values(
            this.customerServices.getCustomerList$().get()
        );

        return deliveryFormBaseSchema.extend({
            customerId: deliveryFormBaseSchema.shape.code.refine(
                (value) => customerList.some(
                    (customer) => customer.id === value
                ),
                "Code client inexistant"
            )
        });
    }
    
    public getCustomerOptions = async (query: string): Promise<Customer[]>  => {
        const customerList = Object.values(
            this.customerServices.getCustomerList$().get()
        );
        
        return customerList.filter(customer => 
            customer.name.toLowerCase().startsWith(query.toLowerCase())
            ||
            customer.code.toLowerCase().startsWith(query.toLowerCase())
        );
    }
}