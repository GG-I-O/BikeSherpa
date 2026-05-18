import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import Delivery from "@/deliveries/models/Delivery";
import DateToolbox from "@/services/DateToolbox";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import {Step} from "@/steps/models/Step";
import unassignedCourierDisplay from "@/deliveries/data/unassignedCourierDisplay";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import DefaultDropdownOption from "@/deliveries/data/defaultDropdownOption";

export default class DeliveryListViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly stepServices: IStepServices;
    private readonly deliveryMapper: IDeliveryMapper;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(StepServiceIdentifier.Services) stepServices: IStepServices,
        @inject(DeliveryServiceIdentifier.Mapper) deliveryMapper: IDeliveryMapper
    ) {
        this.deliveryServices = deliveryServices;
        this.stepServices = stepServices;
        this.deliveryMapper = deliveryMapper;
    }

    public getFilteredDeliveries = (dateFilter: Date | undefined): DeliveryToDisplay[] => {
        if (!this.deliveryServices)
            return [];

        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());

        // Remove all deliveries that have no steps for the filter asked
        const filteredDeliveries = deliveries.filter((delivery) => {
            if (!delivery.steps)
                return false;
            return delivery.steps.some((step) => DateToolbox.dateFilterFunction(dateFilter, new Date(step.estimatedDeliveryDate)));
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
            return this.deliveryMapper.DeliveryToDeliveryToDisplay(delivery);
        });
    }

    public getFilteredStepList = (dateFilter: Date | undefined, courierFilter: string[]): StepToDisplay[] => {
        if (!this.deliveryServices)
            return [];

        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());

        const filteredDeliveries: DeliveryToDisplay[] = [];

        deliveries.forEach((delivery) => {
            const filteredSteps: Step[] = [];
            delivery.steps.forEach((step) => {
                if (
                    DateToolbox.dateFilterFunction(dateFilter, new Date(step.estimatedDeliveryDate))
                    &&
                    (
                        courierFilter.some(courier => courier === DefaultDropdownOption[0].value)
                        ||
                        courierFilter.some(courier => courier === step.courierId))
                ) {
                    filteredSteps.push(step);
                }
            });
            filteredDeliveries.push(this.deliveryMapper.DeliveryToDeliveryToDisplay(
                {
                    ...delivery,
                    steps: filteredSteps
                }));
        });

        return filteredDeliveries.flatMap(delivery => delivery.steps).sort((stepA, stepB) => {
            // Unassigned comes last
            if (stepA.courierCode === unassignedCourierDisplay && stepB.courierCode !== unassignedCourierDisplay) return 1;
            if (stepA.courierCode !== unassignedCourierDisplay && stepB.courierCode === unassignedCourierDisplay) return -1;
            
            // Normal sorting
            if (stepA.courierCode !== stepB.courierCode)
                return stepA.courierCode.localeCompare(stepB.courierCode);
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