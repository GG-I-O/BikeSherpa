import {DropdownOptions} from "@/models/DropdownOptions";

export interface IDropdownOptions {
    GetOptions(): Promise<Record<string, DropdownOptions[]>>;
}