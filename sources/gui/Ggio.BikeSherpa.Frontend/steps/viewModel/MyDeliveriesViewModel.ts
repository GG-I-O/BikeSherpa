import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import Delivery from "@/deliveries/models/Delivery";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";

export default class MyDeliveriesViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly deliveryMapper: IDeliveryMapper;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(DeliveryServiceIdentifier.Mapper) deliveryMapper: IDeliveryMapper
    ) {
        this.deliveryServices = deliveryServices;
        this.deliveryMapper = deliveryMapper;
    }
    
    public loadMyDeliveries = (date: Date): void => {
        const rawDate = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0));
        this.deliveryServices.loadMyDeliveries(rawDate.toISOString());
    }

    public getSteps = (): StepToDisplay[] => {
        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());

        const deliveriesToDisplay: DeliveryToDisplay[] = deliveries.map((delivery) => {
            return this.deliveryMapper.DeliveryToDeliveryToDisplay(delivery);
        });
        
        return deliveriesToDisplay.flatMap(delivery => delivery.steps).sort((stepA, stepB) => {
            return (
                new Date(stepA.estimatedIsoDate).valueOf()
                -
                new Date(stepB.estimatedIsoDate).valueOf()
            );
        });
    }
}