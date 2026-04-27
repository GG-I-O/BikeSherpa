namespace Ggio.BikeSherpa.Backend.Services.Notification;

public interface IResourceNotificationService
{
     Task NotifyResourceChangeToGroup(string dataType, NotificationOperation operation, string resourceId, Guid? operationId = null);
}
