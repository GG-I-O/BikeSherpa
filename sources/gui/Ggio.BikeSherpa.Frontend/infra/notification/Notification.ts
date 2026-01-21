export type ResourceNotification = {
    dataType: string,
    operation: ResourceOperation,
    id: string;
    operationId: string
}

export enum ResourceOperation {
    POST,
    PUT,
    DELETE
}