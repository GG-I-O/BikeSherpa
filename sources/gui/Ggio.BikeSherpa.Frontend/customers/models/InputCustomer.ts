import { Address } from "@/models/Address";

export default class InputCustomer {
    public name: string;
    public address: Address;
    public code: string;
    public phoneNumber: string;
    public email: string;
    public siret?: string | null;
    public vatNumber?: string | null;

    public constructor(
        name: string, address: Address, code: string, phone: string, email: string, siret?: string, vatNumber?: string
    ) {
        this.name = name;
        this.address = address;
        this.code = code;
        this.phoneNumber = phone;
        this.email = email;
        this.siret = siret;
        this.vatNumber = vatNumber;
    }
}