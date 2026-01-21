import { Address } from "@/models/Address";

export interface IAddressService {
    openAddressInMaps(address: string): void;
    fetchAddress(text: string): Promise<Address[] | null>;
}