import * as Crypto from 'expo-crypto';
import { Step } from "@/steps/models/Step";
import Customer from "@/customers/models/Customer";
import PriceDetail from "../../models/PriceDetail";
import { DeliveryPacking } from "./DeliveryPacking";
import ValidationOptions from "@/models/ValidationOptions";
import InputDelivery from "./InputDelivery";
import Storable from "@/models/Storable";

export class Delivery extends InputDelivery implements Storable {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;

    public constructor(
        code: string,
        customer: Customer,
        totalPrice: number,
        reportId: string,
        packing: DeliveryPacking,
        validationOptions: ValidationOptions,
        steps: Step[] = [],
        priceDetails: PriceDetail[] = []
    ) {
        super(code, customer, totalPrice, reportId, packing, validationOptions, steps, priceDetails);
        this.id = Crypto.randomUUID();
    }
}