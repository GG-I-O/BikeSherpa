import Delivery from "@/deliveries/models/Delivery";
import * as Crypto from "expo-crypto";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import {inject} from "inversify";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import PublicDeliveryEstimatedValue from "@/deliveries/models/PublicDeliveryEstimatedValue";

export default class PublicDeliveryFormViewModel {
    private readonly publicDeliveryService: IPublicDeliveryService;

    public constructor(
        @inject(DeliveryServiceIdentifier.PublicServices) publicDeliveryService: IPublicDeliveryService,
    ) {
        this.publicDeliveryService = publicDeliveryService;
    }

    public onSubmit = (delivery: PublicDeliveryFormValues): void => {
        const deliveryObject = this.mapFormValuesToObject(delivery);
    console.log(deliveryObject);
        publicDeliveryStore$.delivery.set(deliveryObject);
    }

    public getEstimatedValue = async (delivery: PublicDeliveryFormValues): Promise<PublicDeliveryEstimatedValue> => {
        const deliveryObject = this.mapFormValuesToObject(delivery);
        return await this.publicDeliveryService.getEstimatedValue(deliveryObject);
    }

    private mapFormValuesToObject = (delivery: PublicDeliveryFormValues): Delivery => {

        return {
            ...delivery,
            code: "",
            customerReference: "",
            contractDate: new Date().toISOString(),
            customerId: Crypto.randomUUID(),
            details: [""],
            discount: 0,
            discountReason: '',
            extraCost: 0,
            extraCostReason: '',
            status: 0,
            id: Crypto.randomUUID(),
            operationId: Crypto.randomUUID(),
            steps: delivery.steps.map(step => {
                return {
                    ...step,
                    id: Crypto.randomUUID(),
                    order: 0,
                    completed: false,
                    stepZone: '',
                    courierId: null,
                    comment: step.comment ?? null,
                    courierComment: step.courierComment ?? null,
                    distance: 0,
                    attachmentFiles: [],
                    realDeliveryDate: null,
                    estimatedDeliveryDate: delivery.startDate,
                    receiver: '',
                    stepAddress: {
                        ...step.stepAddress,
                        name: step.contactName ?? '',
                        phone: step.contactPhone ?? null
                    }
                }
            })
        };
    }
}