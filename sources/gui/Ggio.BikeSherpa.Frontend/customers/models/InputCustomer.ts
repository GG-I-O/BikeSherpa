import { Address } from "@/models/Address";
import CustomerOptions from "./CustomerOptions";

export default class InputCustomer {
    public name: string;
    public address: Address;
    public code: string;
    public phone: string;
    public email: string;
    public siret?: number;
    public comment?: string;
    public options: CustomerOptions;

    public constructor(
        name: string, address: Address, code: string, phone: string, email: string, options: CustomerOptions, siret?: number, comment?: string
    ) {
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