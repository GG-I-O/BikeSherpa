import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import Delivery from "@/deliveries/models/Delivery";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import {StepDisplayForCourier} from "@/steps/models/StepDisplayForCourier";
import {DeliveryDisplayForCourier} from "@/deliveries/models/DeliveryDisplayForCourier";

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

    public getSteps = (): StepDisplayForCourier[] => {
        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());

        const deliveriesToDisplay: DeliveryDisplayForCourier[] = deliveries.map((delivery) => {
            return this.deliveryMapper.DeliveryToDeliveryDisplayForCourier(delivery);
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