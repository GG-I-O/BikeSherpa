using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Ggio.BikeSherpa.Backend.Services.Notification;

public class ResourceNotificationService(IHubContext<ResourceNotificationHub> hubContext, ILogger logger): IResourceNotificationService
{
     public async Task NotifyResourceChangeToGroup(string resourceName, ResourceOperation operation, string id, Guid? operationId = null)
     {
          logger.Debug("Sending notification for {0} {1} {2}", resourceName, operation, id);
        
          var notification = new ResourceNotification(resourceName, operation, id, operationId);
          await hubContext.Clients.Group(resourceName).SendAsync("notification", notification);
     }
}
