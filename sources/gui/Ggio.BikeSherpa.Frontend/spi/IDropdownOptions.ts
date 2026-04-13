import {DropdownOptions} from "@/models/DropdownOptions";

export interface IDropdownOptions<T> {
    GetOptions(): Promise<Record<string, DropdownOptions[]>>;
}