import * as zod from 'zod';
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { inject } from "inversify";
import { UseFormReset } from "react-hook-form";
import { IDeliveryService } from '@/spi/DeliverySPI';
import { InputDelivery } from '../models/InputDelivery';
import { Delivery } from '../models/Delivery';
import { customerSchema } from '@/customers/models/Customer';
import { detailSchema } from '../models/DeliveryDetail';
import { stepSchema } from '@/steps/models/Step';
import { DeliveryPacking } from '../models/DeliveryPacking';

export default class NewDeliveryFormViewModel {
    private deliveryServices: IDeliveryService;
    private resetCallback?: UseFormReset<InputDelivery>;

    constructor(
        @inject(ServicesIdentifiers.DeliveryServices) deliveryServices: IDeliveryService
    ) {
        this.deliveryServices = deliveryServices;
    }

    public onSubmit = (delivery: InputDelivery): void => {
        this.deliveryServices.createDelivery(delivery);
        if (this.resetCallback) {
            this.resetCallback(); // Clear form after submission
        }
    };

    public setResetCallback(reset?: UseFormReset<InputDelivery>) {
        this.resetCallback = reset;
    }

    public getNewDeliverySchema(deliveryList: Delivery[]) {
        return zod.object({
            code: zod
                .string()
                .trim()
                .min(1, "Code requis")
                .max(3, "Code trop long")
                .refine((value) => !deliveryList.some((delivery) => delivery.code === value), "Le code doit être unique"),
            customer: customerSchema,
            totalPrice: zod
                .number(),
            reportId: zod
                .string(),
            steps: stepSchema,
            details: detailSchema,
            packing: zod.nativeEnum(DeliveryPacking),
        });
    }
}