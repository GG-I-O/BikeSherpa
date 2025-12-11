export type ResourceNotification = {
    dataType: string,
    operation: ResourceOperation,
    id: string
}

export enum ResourceOperation
{
    POST,
    PUT,
    DELETE
}