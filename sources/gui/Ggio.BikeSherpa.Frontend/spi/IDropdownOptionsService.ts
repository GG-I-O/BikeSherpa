import {DropdownOptions} from "@/models/DropdownOptions";

export interface IDropdownOptionsService {
    GetOptions(): Promise<Record<string, DropdownOptions[]>>;
    GetPackingLabel(packing: string): string;
}