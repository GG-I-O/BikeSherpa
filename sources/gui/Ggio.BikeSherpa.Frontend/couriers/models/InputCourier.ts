import { Address } from "@/models/Address";

export default class InputCourier {
    public firstName: string;
    public lastName: string;
    public code: string;
    public phoneNumber: string;
    public address: Address;
    public email?: string | null;

    public constructor(
        firstName: string, lastName: string, code: string, phone: string, address: Address, email?: string
    ) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.code = code;
        this.phoneNumber = phone;
        this.address = address;
        this.email = email;
    }
}