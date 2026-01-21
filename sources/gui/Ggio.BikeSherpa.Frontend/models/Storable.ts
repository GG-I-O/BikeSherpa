import { Identifiable } from "./Identifiable";

type Storable = {
    createdAt?: string,
    updatedAt?: string,
    operationId?: string
} & Identifiable<string>

export default Storable;