import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICourierService} from "@/spi/CourierSPI";
import {ICustomerService} from "@/spi/CustomerSPI";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {inject} from "inversify";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import Delivery from "@/deliveries/models/Delivery";
import DeliveryMapper from "@/deliveries/services/DeliveryMapper";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";

export default class DeliveryDetailViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly courierServices: ICourierService;
    private readonly customerServices: ICustomerService;
    private readonly dropdownOptionsService: IDropdownOptionsService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService,
        @inject(DeliveryServiceIdentifier.DropdownOptionsService) dropdownOptionsService: IDropdownOptionsService,
    ) {
        this.deliveryServices = deliveryServices;
        this.courierServices = courierServices;
        this.customerServices = customerServices;
        this.dropdownOptionsService = dropdownOptionsService;
    }

    public getDelivery = (id: string): DeliveryToDisplay => {
        const delivery: Delivery = this.deliveryServices.getDelivery$(id).get();

        return DeliveryMapper.DeliveryToDeliveryToDisplay(
            delivery,
            (id: string) => this.customerServices.getCustomer$(id).get().name,
            (id: string) => this.courierServices.getCourier$(id).get().code,
            this.dropdownOptionsService.GetPackingLabel,
        );
    }
}