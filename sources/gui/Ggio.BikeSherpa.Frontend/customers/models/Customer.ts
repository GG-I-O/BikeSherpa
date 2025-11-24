import CustomerOptions from "./CustomerOptions";
import * as Crypto from 'expo-crypto';

export default class Customer {
    public readonly id: string;
    public name: string;
    public address: string;
    public code: string;
    public phone: string;
    public email: string;
    public siret?: string;
    public comment?: string;
    public options: CustomerOptions;

    public constructor(
        name: string, address: string, code: string, phone: string, email: string, options: CustomerOptions, siret?: string, comment?: string
    ) {
        this.id = Crypto.randomUUID();
        this.name = name;
        this.address = address;
        this.code = code;
        this.phone = phone;
        this.email = email;
        this.options = options;
        this.siret = siret;
        this.comment = comment;
    }
}