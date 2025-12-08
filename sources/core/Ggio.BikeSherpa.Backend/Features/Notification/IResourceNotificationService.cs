namespace Ggio.BikeSherpa.Backend.Features.Notification;

public interface IResourceNotificationService
{
     Task NotifyResourceChangeToGroup(string dataType, ResourceOperation operation, string resourceId);
}
