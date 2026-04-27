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
import DeliveryMapper from "@/deliveries/services/DeliveryMapper";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import {Step} from "@/steps/models/Step";

export default class DeliveryListViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly stepServices: IStepServices;
    private readonly courierServices: ICourierService;
    private readonly customerServices: ICustomerService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(StepServiceIdentifier.Services) stepServices: IStepServices,
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService,
    ) {
        this.deliveryServices = deliveryServices;
        this.stepServices = stepServices;
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
                return (
                    DateToolbox.dateFilterFunction(dateFilter, new Date(step.estimatedDeliveryDate))
                    &&
                    ((courierFilter === '' && step.courierId === null) || courierFilter === step.courierId)
                );
            });
        });

        const sortedDeliveries: Delivery[] = [...filteredDeliveries].sort((deliveryA, deliveryB) => {
            if (!deliveryA.steps || !deliveryB.steps)
                return 0;

            return (
                new Date(deliveryA.steps[0].estimatedDeliveryDate).valueOf()
                -
                new Date(deliveryB.steps[0].estimatedDeliveryDate).valueOf()
            );
        });

        return sortedDeliveries.map((delivery) => {
            return DeliveryMapper.DeliveryToDeliveryToDisplay(
                delivery,
                (id: string) => {
                    const customer = this.customerServices.getCustomer$(id).get();
                    return customer?.name ?? "";
                },
                (id: string) => {
                    const courier = this.courierServices.getCourier$(id).get();
                    return courier?.code ?? "";
                }
            );
        });
    }

    public getFilteredStepList = (dateFilter: string, courierFilter: string): StepToDisplay[] => {
        if (!this.deliveryServices || !this.courierServices)
            return [];

        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());

        const filteredDeliveries: DeliveryToDisplay[] = [];

        deliveries.forEach((delivery) => {
            const filteredSteps: Step[] = [];
            delivery.steps.forEach((step) => {
                if (
                    DateToolbox.dateFilterFunction(dateFilter, new Date(step.estimatedDeliveryDate))
                    &&
                    ((courierFilter === '' && step.courierId === null) || courierFilter === step.courierId)
                ) {
                    filteredSteps.push(step);
                }
            });
            filteredDeliveries.push(DeliveryMapper.DeliveryToDeliveryToDisplay(
                {
                    ...delivery,
                    steps: filteredSteps
                },
                (id: string) => {
                    const customer = this.customerServices.getCustomer$(id).get();
                    return customer?.name ?? "";
                },
                (id: string) => {
                    const courier = this.courierServices.getCourier$(id).get();
                    return courier?.code ?? "";
                }
            ));
        });

        return filteredDeliveries.flatMap(delivery => delivery.steps).sort((stepA, stepB) => {
            return (
                new Date(stepA.estimatedIsoDate).valueOf()
                -
                new Date(stepB.estimatedIsoDate).valueOf()
            );
        });
    }

    public assignCourier = (steps: StepToDisplay[], courierId: string) => {
        steps.forEach(step => {
            this.stepServices.assignCourier(step.id, courierId);
        });
    }

    public unassignCourier = (steps: StepToDisplay[]) => {
        steps.forEach(step => {
            this.stepServices.unassignCourier(step.id);
        });
    }
}