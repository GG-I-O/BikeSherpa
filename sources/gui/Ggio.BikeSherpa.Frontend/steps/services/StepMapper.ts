import {Step} from "@/steps/models/Step";
import DateToolbox from "@/services/DateToolbox";
import Delivery from "@/deliveries/models/Delivery";

export default class StepMapper {
    public static StepToStepToDisplay(delivery: Delivery, step: Step, getCourierCode: (id: string) => string, getPackingLabel: (packing: string) => string) {
        return {
            id: step.id,
            deliveryId: delivery.id,
            deliveryCode: delivery.code,
            deliveryLimitDate: !delivery.limitDate ? '???' : DateToolbox.getFormattedTimeFromISO(new Date(delivery.limitDate).toISOString()),
            type: step.stepType,
            order: step.order,
            completed: step.completed,
            address: step.stepAddress,
            courierCode: step.courierId ? getCourierCode(step.courierId) : "???",
            comment: step.comment ?? '',
            packing: getPackingLabel(delivery.packingSize),
            deliveryDate: DateToolbox.getFormattedDateFromISO(new Date(delivery.startDate).toISOString()),
            deliveryTime: DateToolbox.getFormattedTimeFromISO(new Date(delivery.startDate).toISOString()),
            estimatedIsoDate: step.estimatedDeliveryDate,
            estimatedDate: DateToolbox.getFormattedDateFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
            estimatedTime: DateToolbox.getFormattedTimeFromISO(new Date(step.estimatedDeliveryDate).toISOString()),
        }
    }
}