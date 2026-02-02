import * as Crypto from "expo-crypto";
import InputCourier from "./InputCourier";
import Storable from "@/models/Storable";
import { Address } from "@/models/Address";
import { HateoasLinks, Link } from "@/models/HateoasLink";
import { z } from "zod";
import { schemas } from "@/infra/openAPI/client";

export default class Courier extends InputCourier implements Storable, HateoasLinks {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public operationId?: string;
    public links?: Link[];

    public constructor(
        firstName: string, lastName: string, code: string, phoneNumber: string, email: string, address: Address
    ) {
        super(firstName, lastName, code, phoneNumber, email, address);
        this.id = Crypto.randomUUID();
        this.links = [];
    }
};

export type CourierCrud = z.infer<typeof schemas.CourierCrud>;

export type CourierDto = z.infer<typeof schemas.CourierDto>;