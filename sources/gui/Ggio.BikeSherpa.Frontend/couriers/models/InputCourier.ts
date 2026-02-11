import { Address } from "@/models/Address";

export default class InputCourier {
    public firstName: string;
    public lastName: string;
    public code: string;
    public phoneNumber: string;
    public email: string;
    public address: Address;

    public constructor(
        firstName: string, lastName: string, code: string, phoneNumber: string, email: string, address: Address
    ) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.code = code;
        this.phoneNumber = phoneNumber;
        this.email = email;
        this.address = address;
    }
}