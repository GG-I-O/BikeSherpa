import * as zod from 'zod';

export type Address = {
    name: string;
    streetInfo: string;
    complement?: string;
    postcode: string;
    city: string;
}

export const addressSchema = zod.object({
    name: zod.string(),
    streetInfo: zod.string(),
    complement: zod.string(),
    postcode: zod.string(),
    city: zod.string(),
}).partial({ complement: true })