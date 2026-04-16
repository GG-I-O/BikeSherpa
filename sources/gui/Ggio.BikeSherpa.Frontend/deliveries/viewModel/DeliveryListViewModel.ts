import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICourierService} from "@/spi/CourierSPI";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import Delivery from "@/deliveries/models/Delivery";
import DateToolbox from "@/services/DateToolbox";
import {Step} from "@/steps/models/Step";

export default class DeliveryListViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly courierServices: ICourierService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
    ) {
        this.deliveryServices = deliveryServices;
        this.courierServices = courierServices;
    }

    public getFilteredDeliveries = (dateFilter: string, courierFilter: string): Delivery[] => {
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

        // Return sorted deliveries
        return [...filteredDeliveries].sort((deliveryA, deliveryB) => {
            if (!deliveryA.steps || !deliveryB.steps)
                return 0;
            return (
                new Date(deliveryA.steps[0].estimatedDeliveryDate).getTime()
                -
                new Date(deliveryB.steps[0].estimatedDeliveryDate).getTime()
            );
        });
    }

    public getFilteredStepList = (dateFilter: string, courier: string): Step[] => {
        return this.getFilteredDeliveries(dateFilter, courier)
            .flatMap((delivery) => delivery.steps ?? []);
    }
}