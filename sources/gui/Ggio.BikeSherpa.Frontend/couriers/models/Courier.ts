import * as zod from 'zod';
import * as Crypto from 'expo-crypto';

export default class Courier {
    public readonly id: string;
    public code: string;
    public firstName: string;
    public lastName: string;
    public phone: string;
    public email: string;

    public constructor(
        code: string,
        firstName: string,
        lastName: string,
        phone: string,
        email: string
    ) {
        this.id = Crypto.randomUUID();
        this.code = code;
        this.firstName = firstName;
        this.lastName = lastName;
        this.phone = phone;
        this.email = email;
    }
}

export const courierSchema = zod.object({
    code: zod.string(),
    firstName: zod.string(),
    lastName: zod.string(),
    phone: zod.string(),
    email: zod.string(),
});