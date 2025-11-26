import ValidationOptions from "@/models/ValidationOptions";
import PriceDetail from "@/models/PriceDetail";

export default class InputCustomer {
    public name: string;
    public address: string;
    public code: string;
    public phone: string;
    public email: string;
    public siret?: number;
    public comment?: string;
    public options: ValidationOptions;
    public priceDetails: PriceDetail[];

    public constructor(
        name: string,
        address: string,
        code: string,
        phone: string,
        email: string,
        options: ValidationOptions,
        priceDetails: PriceDetail[] = [],
        siret?: number, comment?: string
    ) {
        this.name = name;
        this.address = address;
        this.code = code;
        this.phone = phone;
        this.email = email;
        this.options = options;
        this.priceDetails = priceDetails;
        this.siret = siret;
        this.comment = comment;
    }
}