import Delivery, {DeliveryDto} from "../models/Delivery";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import DateToolbox from "@/services/DateToolbox";

export default class DeliveryMapper {
    public static DeliveryDtoToDelivery(deliveryDto: DeliveryDto) {
        const deliveryCrud = deliveryDto.data;
        let delivery: Delivery = {
            ...deliveryCrud,
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
        getCourierCode: (id: string) => string
        ): DeliveryToDisplay {
        return {
            id: delivery.id,
            code: delivery.code,
            customerName: getCustomerName(delivery.customerId),
            urgency: delivery.urgency,
            startDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
            startTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
            steps: delivery.steps?.map((step) => ({
                id: step.id,
                deliveryId: delivery.id,
                deliveryCode: delivery.code,
                deliveryUrgency: delivery.urgency,
                type: step.stepType,
                order: step.order,
                completed: step.completed,
                address: step.stepAddress,
                courierCode: step.courierId ? getCourierCode(step.courierId) : undefined,
                comment: step.comment ?? '',
                deliveryDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
                deliveryTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
                estimatedIsoDate: step.estimatedDeliveryDate,
                estimatedDate: DateToolbox.getFormattedDateFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
                estimatedTime: DateToolbox.getFormattedTimeFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
            }))
        }
    }
}