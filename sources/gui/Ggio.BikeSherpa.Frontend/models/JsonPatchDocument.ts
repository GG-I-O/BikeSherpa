export type JsonPatchOperation = {
    path: string;
    op: string;
    value: unknown;
};

export default class JsonPatchDocument {
    private operations: JsonPatchOperation[] = [];

    public addOperation(path: string, op: string, value: unknown): void {
        this.operations.push({
            path,
            op,
            value
        });
    }

    public getOperations(): JsonPatchOperation[] {
        return this.operations;
    }
}