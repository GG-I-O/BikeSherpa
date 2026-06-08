import {Step} from "@/steps/models/Step";
import DateToolbox from "@/services/DateToolbox";
import Delivery from "@/deliveries/models/Delivery";
import {inject, injectable} from "inversify";
import IStepMapper from "@/steps/spi/IStepMapper";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";
import {ICourierService} from "@/spi/CourierSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import unassignedCourierDisplay from "@/deliveries/data/unassignedCourierDisplay";

@injectable()
export default class StepMapper implements IStepMapper {
    private readonly courierServices: ICourierService;
    private readonly dropdownOptionsService: IDropdownOptionsService;

    constructor(
        @inject(ServicesIdentifiers.CourierServices) courierServices: ICourierService,
        @inject(ServicesIdentifiers.DropdownOptionsService) dropdownOptionsService: IDropdownOptionsService,
    ) {
        this.courierServices = courierServices;
        this.dropdownOptionsService = dropdownOptionsService;
    }
    
    public StepToStepToDisplay = (delivery: Delivery, step: Step)=> {
        return {
            id: step.id,
            deliveryId: delivery.id,
            deliveryCode: delivery.code,
            deliveryLimitDate: !delivery.limitDate ? unassignedCourierDisplay : DateToolbox.getFormattedTimeFromISO(new Date(delivery.limitDate).toISOString()),
            type: step.stepType,
            order: step.order,
            completed: step.completed,
            address: step.stepAddress,
            courierCode: step.courierId ? this.courierServices.getCourier$(step.courierId).get().code : unassignedCourierDisplay,
            comment: step.comment ?? '',
            courierComment: step.courierComment ?? '',
            packing: this.dropdownOptionsService.GetPackingLabel(delivery.packingSize),
            deliveryDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
            deliveryTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
            estimatedIsoDate: step.estimatedDeliveryDate,
            estimatedDate: DateToolbox.getFormattedDateFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
            estimatedTime: DateToolbox.getFormattedTimeFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
            distance: Math.round(step.distance * 100) / 100,
            notBilled: step.notBilled,
            attachmentFilePaths: step.attachmentFilePaths ?? [],
        }
    }
}