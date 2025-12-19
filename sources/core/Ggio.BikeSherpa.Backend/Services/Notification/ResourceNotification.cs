namespace Ggio.BikeSherpa.Backend.Services.Notification;

public class ResourceNotification(string dataType, ResourceOperation operation, string id, Guid? operationId = null)
{
     public string DataType { get; set; } = dataType;
     public ResourceOperation Operation { get; set; } = operation;
     public string Id { get; set; } = id;
     public Guid? OperationId { get; set; } = operationId;
}
