import Delivery, {DeliveryDto} from "../models/Delivery";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import DateToolbox from "@/services/DateToolbox";
import StepMapper from "@/steps/services/StepMapper";

export default class DeliveryMapper {
    public static DeliveryDtoToDelivery(deliveryDto: DeliveryDto) {
        const deliveryCrud = deliveryDto.data;
        let delivery: Delivery = {
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
        }
        return delivery;
    }

    public static DeliveryToDeliveryToDisplay(
        delivery: Delivery,
        getCustomerName: (id: string) => string,
        getCourierCode: (id: string) => string,
        getPackingLabel: (packing: string) => string
        ): DeliveryToDisplay {
        return {
            id: delivery.id,
            code: delivery.code,
            customerName: getCustomerName(delivery.customerId),
            urgency: delivery.urgency,
            totalPrice: `${delivery.totalPrice ?? 0}€`,
            startDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
            startTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
            limitTime: !delivery.limitDate ? '???' : DateToolbox.getFormattedTimeFromISO(new Date(delivery.limitDate).toISOString()),
            steps: delivery.steps?.map((step) => (
                StepMapper.StepToStepToDisplay(delivery, step, getCourierCode, getPackingLabel)
            ))
        }
    }
}