import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {ICourierService} from "@/spi/CourierSPI";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {inject} from "inversify";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import Delivery from "@/deliveries/models/Delivery";
import {Step} from "@/steps/models/Step";
import StepMapper from "@/steps/services/StepMapper";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";

export default class StepDetailViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly courierServices: ICourierService;
    private readonly dropdownOptionsService: IDropdownOptionsService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
        @inject(DeliveryServiceIdentifier.DropdownOptionsService) dropdownOptionsService: IDropdownOptionsService
    ) {
        this.deliveryServices = deliveryServices;
        this.courierServices = courierServices;
        this.dropdownOptionsService = dropdownOptionsService;
    }

    public getStep = (stepId: string): StepToDisplay | undefined => {
        if (!this.deliveryServices || !this.courierServices)
            return undefined;

        const deliveries: Delivery[] = Object.values(this.deliveryServices.getDeliveryList$().get());
        let delivery: Delivery | undefined;
        let step: Step | undefined;
        deliveries.forEach(d => {
            const s = d.steps.find((step) => step.id === stepId);
            if (s) {
                delivery = d;
                step = s;
                return;
            }
        });
        if (!delivery || !step)
            return undefined;

        return StepMapper.StepToStepToDisplay(
            delivery,
            step,
            (id: string) => {
                const courier = this.courierServices.getCourier$(id).get();
                return courier?.code ?? "";
            },
            this.dropdownOptionsService.GetPackingLabel
        );
    }
}