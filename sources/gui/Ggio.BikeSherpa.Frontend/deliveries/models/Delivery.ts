import { Identifiable } from "@/models/Identifiable";
import * as Crypto from "expo-crypto";
import DeliveryDetail from "./DeliveryDetail";
import { PackageSize } from "./PackageSize";
import { InputDelivery } from "./InputDelivery";
import { Link } from "@/models/HateoasLink";
import { z } from "zod";
import { schemas } from "@/infra/openAPI/client";
import { DeliveryStatus } from "./DeliveryStatus";

export class Delivery extends InputDelivery implements Identifiable<string> {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public links?: Link[];

    public constructor(deliveryStatus: DeliveryStatus, code: string, customerId: string, totalPrice: number, reportId: string, stepIds: string[] = [], details: DeliveryDetail[] = [], packing: PackageSize) {
        super(deliveryStatus, code, customerId, totalPrice, reportId, stepIds, details, packing);
        this.id = Crypto.randomUUID();
        this.links = [];
    }
}

export type DeliveryCrud = z.infer<typeof schemas.DeliveryCrud>;

export type DeliveryDto = z.infer<typeof schemas.DeliveryDto>;