import CustomerOptions from "./CustomerOptions";
import * as Crypto from 'expo-crypto';
import InputCustomer from "./InputCustomer";
import Storable from "@/models/Storable";
import { Address } from "@/models/Address";
import { HateoasLinks, Link } from "@/models/HateoasLink";

export default class Customer extends InputCustomer implements Storable, HateoasLinks {
    // Storable
    public readonly id: string;
    public createdAt?: string;
    public updatedAt?: string;
    public links: Link[];

    public constructor(
        name: string, address: Address, code: string, phone: string, email: string, options: CustomerOptions, siret?: number, comment?: string
    ) {
        super(name, address, code, phone, email, options, siret, comment);
        this.id = Crypto.randomUUID();
        this.links = [];
    }
};