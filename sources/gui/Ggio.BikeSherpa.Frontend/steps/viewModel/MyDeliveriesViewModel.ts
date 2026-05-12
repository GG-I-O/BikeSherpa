import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {ICourierService} from "@/spi/CourierSPI";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import Delivery from "@/deliveries/models/Delivery";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import DeliveryMapper from "@/deliveries/services/DeliveryMapper";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";

export default class MyDeliveriesViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly courierServices: ICourierService;
    private readonly customerServices: ICustomerService;
    private readonly dropdownOptionsService: IDropdownOptionsService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService,
        @inject(DeliveryServiceIdentifier.DropdownOptionsService) dropdownOptionsService: IDropdownOptionsService
    ) {
        this.deliveryServices = deliveryServices;
        this.courierServices = courierServices;
        this.customerServices = customerServices;
        this.dropdownOptionsService = dropdownOptionsService;
    }
    
    public loadMyDeliveries = (date: Date): void => {
        const rawDate = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0));
        this.deliveryServices.loadMyDeliveries(rawDate.toISOString());
    }

    public getSteps = (): StepToDisplay[] => {
        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());

        const deliveriesToDisplay: DeliveryToDisplay[] = deliveries.map((delivery) => {
            return DeliveryMapper.DeliveryToDeliveryToDisplay(
                delivery,
                (id: string) => {
                    const customer = this.customerServices.getCustomer$(id).get();
                    return customer?.name ?? "";
                },
                (id: string) => {
                    const courier = this.courierServices.getCourier$(id).get();
                    return courier?.code ?? "";
                },
                this.dropdownOptionsService.GetPackingLabel,
            );
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