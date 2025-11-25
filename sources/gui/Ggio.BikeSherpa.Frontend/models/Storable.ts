import { Identifiable } from "./Identifiable";

type Storable = {
    createdAt?: string,
    updatedAt?: string,
} & Identifiable<string>

export default Storable;