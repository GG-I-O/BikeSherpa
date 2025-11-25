import CustomerOptions from "./CustomerOptions";
import * as Crypto from 'expo-crypto';
import InputCustomer from "./InputCustomer";
import Storable from "@/models/Storable";

export default class Customer extends InputCustomer implements Storable {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;

    public constructor(
        name: string, address: string, code: string, phone: string, email: string, options: CustomerOptions, siret?: number, comment?: string
    ) {
        super(name, address, code, phone, email, options, siret, comment);
        this.id = Crypto.randomUUID();
    }
}