import { Address } from "@/models/Address";
import CustomerOptions from "./CustomerOptions";

export default class InputCustomer {
    public name: string;
    public address: Address;
    public code: string;
    public phoneNumber: string;
    public email: string;
    public siret?: number;
    public comment?: string;

    public constructor(
        name: string, address: Address, code: string, phone: string, email: string, siret?: number, comment?: string
    ) {
        this.name = name;
        this.address = address;
        this.code = code;
        this.phoneNumber = phone;
        this.email = email;
        this.siret = siret;
        this.comment = comment;
    }
}