import * as Crypto from 'expo-crypto';
import InputCustomer from "./InputCustomer";
import Storable from "@/models/Storable";
import PriceDetail from "@/models/PriceDetail";
import ValidationOptions from "@/models/ValidationOptions";

export default class Customer extends InputCustomer implements Storable {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;

    public constructor(
        name: string,
        address: string,
        code: string,
        phone: string,
        email: string,
        options: ValidationOptions,
        priceDetails: PriceDetail[] = [],
        siret?: number,
        comment?: string
    ) {
        super(name, address, code, phone, email, options, priceDetails, siret, comment);
        this.id = Crypto.randomUUID();
    }
}