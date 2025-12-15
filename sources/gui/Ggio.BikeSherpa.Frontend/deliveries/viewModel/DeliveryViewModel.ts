import DateToolbox from "@/services/DateToolbox";
import { Delivery } from "../models/Delivery";
import { Step } from "@/steps/models/Step";

class DeliveryViewModel {
    private static instance: DeliveryViewModel;
    private readonly deliveries: Delivery[];

    public static getInstance(): DeliveryViewModel {
        if (!DeliveryViewModel.instance)
            DeliveryViewModel.instance = new DeliveryViewModel();
        return DeliveryViewModel.instance;
    }

    private constructor() {
        this.deliveries = [];
    }

    public getDeliveryList(): Delivery[] {
        return this.deliveries;
    }

    public getDelivery(deliveryId: string): Delivery | undefined {
        return this.deliveries.find((delivery) => delivery.id === deliveryId);
    }

    public getDeliveryByCode(deliveryCode: string): Delivery | undefined {
        return this.deliveries.find((delivery) => delivery.code === deliveryCode);
    }

    public getFilteredDeliveries(dateFilter: string, courier: string): Delivery[] {
        // Today
        const todayStart = new Date();
        todayStart.setHours(0, 0, 0, 0);
        const todayEnd = new Date(todayStart);
        todayEnd.setHours(23, 59, 59, 0);

        // Tomorrow
        const tomorrowStart = new Date(todayStart);
        tomorrowStart.setDate(tomorrowStart.getDate() + 1);
        const tomorrowEnd = new Date(tomorrowStart);
        tomorrowEnd.setHours(23, 59, 59, 0);

        // Week
        const endOfWeek = DateToolbox.getLastDayOfWeek(todayEnd);

        const dateFilterFunction = (dateFilter: string, date: Date): boolean => {
            switch (dateFilter) {
                case '1':
                    return date.valueOf() >= todayStart.valueOf() && date.valueOf() <= todayEnd.valueOf();
                case '2':
                    return date.valueOf() >= tomorrowStart.valueOf() && date.valueOf() <= tomorrowEnd.valueOf();
                case '7':
                    return date.valueOf() >= todayStart.valueOf() && date.valueOf() <= endOfWeek.valueOf();
                default:
                    return date.valueOf() >= todayStart.valueOf();
            }
        }

        const courierFilterFunction = (courierFilter: string, courier: string): boolean => {
            return (courierFilter === 'NONE' && courier === undefined) || courierFilter === courier;
        }

        // Remove all deliveries that have no steps for the filter asked
        const filteredDeliveries = this.deliveries.filter((delivery) => {
            if (!delivery.steps)
                return false;
            return delivery.steps.some((step) => {
                return dateFilterFunction(dateFilter, step.contractDate) &&
                    courierFilterFunction(courier, step.courier ?? 'NONE');
            });
        });

        const sortedDeliveries = [...filteredDeliveries].sort((deliveryA, deliveryB) => {
            if (!deliveryA.steps || !deliveryB.steps)
                return 0;
            return deliveryA.steps[0].contractDate.valueOf() - deliveryB.steps[0].contractDate.valueOf();
        });

        return sortedDeliveries;
    }

    public getFilteredStepList(dateFilter: string, courier: string): Step[] {
        return this.getFilteredDeliveries(dateFilter, courier).flatMap((delivery) => delivery.steps ?? []);
    }

    public getStep(stepId: string): Step | undefined {
        return this.getStepList().find((step) => step.id === stepId);
    }

    public getStepList(): Step[] {
        return this.deliveries.flatMap((delivery) => delivery.steps ?? []);
    }

    public assignSteps(courier: string, steps: Step[]): void {
        steps.forEach((step: Step) => step.courier = courier !== 'NONE' ? courier : undefined);
    }
}

export default function useDeliveryViewModel() {
    return DeliveryViewModel.getInstance();
}