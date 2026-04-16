import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

export default function useDeliveryDataTableRowViewModel() {
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    
    return {
        getCustomerName: (customerId: string) => customerServices.getCustomer$(customerId).get().name  
    };
}