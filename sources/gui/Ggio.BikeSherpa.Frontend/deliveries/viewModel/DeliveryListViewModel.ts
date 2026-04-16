import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICourierService} from "@/spi/CourierSPI";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import Delivery from "@/deliveries/models/Delivery";
import DateToolbox from "@/services/DateToolbox";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {ICustomerService} from "@/spi/CustomerSPI";

export default class DeliveryListViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly courierServices: ICourierService;
    private readonly customerServices: ICustomerService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService,
    ) {
        this.deliveryServices = deliveryServices;
        this.courierServices = courierServices;
        this.customerServices = customerServices;
    }

    public getFilteredDeliveries = (dateFilter: string, courierFilter: string): DeliveryToDisplay[] => {
        if (!this.deliveryServices || !this.courierServices)
            return [];
        
        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());
        
        // Remove all deliveries that have no steps for the filter asked
        const filteredDeliveries = deliveries.filter((delivery) => {
            if (!delivery.steps)
                return false;
            
            return delivery.steps.some((step) => {
                const courier = step.courierId
                    ? this.courierServices.getCourier(step.courierId).code
                    : 'NONE';
                return (
                    DateToolbox.dateFilterFunction(dateFilter, new Date(step.estimatedDeliveryDate))
                    &&
                    ((courierFilter === 'NONE' && courier === undefined) || courierFilter === courier)
                );
            });
        });
        
        const sortedDeliveries: Delivery[] = [...filteredDeliveries].sort((deliveryA, deliveryB) => {
            if (!deliveryA.steps || !deliveryB.steps)
                return 0;
            return (
                new Date(deliveryA.steps[0].estimatedDeliveryDate).getTime()
                -
                new Date(deliveryB.steps[0].estimatedDeliveryDate).getTime()
            );
        });
        
        return sortedDeliveries.map((delivery) => {
            return {
                id: delivery.id,
                code: delivery.code,
                customerName: this.customerServices.getCustomer$(delivery.customerId).get().name,
                urgency: delivery.urgency,
                startDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
                startTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
                steps: delivery.steps?.map((step) => ({
                    id: step.id,
                    type: step.stepType,
                    completed: step.completed,
                    address: step.stepAddress,
                    courierCode: step.courierId ? this.courierServices.getCourier$(step.courierId).get().code : undefined,
                    comment: step.comment ?? '',
                    estimatedDate: DateToolbox.getFormattedDateFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
                    estimatedTime: DateToolbox.getFormattedTimeFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
                }))
            }
        });
    }

    public getFilteredStepList = (dateFilter: string, courier: string): StepToDisplay[] => {
        return this.getFilteredDeliveries(dateFilter, courier)
            .flatMap((delivery) => delivery.steps ?? []);
    }
}