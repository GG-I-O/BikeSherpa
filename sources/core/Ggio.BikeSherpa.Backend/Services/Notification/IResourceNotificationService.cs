namespace Ggio.BikeSherpa.Backend.Services.Notification;

public interface IResourceNotificationService
{
     Task NotifyResourceChangeToGroup(string dataType, ResourceOperation operation, string resourceId, Guid? operationId = null);
}
