import { Address } from "@/models/Address";

export interface IAddressService {
    openAdressInMaps(address: string): void;
    fetchAddress(text: string): Promise<Address[] | null>;
}