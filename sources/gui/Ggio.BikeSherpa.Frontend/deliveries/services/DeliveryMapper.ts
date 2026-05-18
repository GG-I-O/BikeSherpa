import Delivery, {DeliveryDto} from "../models/Delivery";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import DateToolbox from "@/services/DateToolbox";
import {inject, injectable} from "inversify";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import IStepMapper from "@/steps/spi/IStepMapper";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";

@injectable()
export default class DeliveryMapper implements IDeliveryMapper {
    private readonly customerServices: ICustomerService;
    private readonly stepMapper: IStepMapper;

    constructor(
        @inject(ServicesIdentifiers.CustomerServices) customerServices: ICustomerService,
        @inject(StepServiceIdentifier.Mapper) stepMapper: IStepMapper
    ) {
        this.customerServices = customerServices;
        this.stepMapper = stepMapper;
    }
    
    public DeliveryDtoToDelivery(deliveryDto: DeliveryDto): Delivery {
        const deliveryCrud = deliveryDto.data;
        return {
            ...deliveryCrud,
            limitDate: deliveryCrud.limitDate,
            steps: deliveryDto.data.steps.map((step => {
                return {
                    ...step.data,
                    stepAddress: {
                        ...step.data.stepAddress,
                        fullAddress: `${step.data.stepAddress.streetInfo} ${step.data.stepAddress.postcode} ${step.data.stepAddress.city}`,
                    },
                    links: step.links ?? []
                }
            })),
            links: deliveryDto.links ?? []
        };
    }

    public DeliveryToDeliveryToDisplay(delivery: Delivery): DeliveryToDisplay {
        return {
            id: delivery.id,
            code: delivery.code,
            customerName: this.customerServices.getCustomer$(delivery.customerId).get().name,
            urgency: delivery.urgency,
            totalPrice: delivery.totalPrice ?? 0,
            startDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
            startTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
            limitTime: !delivery.limitDate ? '???' : DateToolbox.getFormattedTimeFromISO(new Date(delivery.limitDate).toISOString()),
            steps: delivery.steps?.map((step) => (
                this.stepMapper.StepToStepToDisplay(delivery, step)
            ))
        }
    }
}