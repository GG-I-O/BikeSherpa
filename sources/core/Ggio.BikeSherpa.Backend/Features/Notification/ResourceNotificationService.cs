using Microsoft.AspNetCore.SignalR;

namespace Ggio.BikeSherpa.Backend.Features.Notification;

public class ResourceNotificationService(IHubContext<ResourceNotificationHub> hubContext): IResourceNotificationService
{
     private readonly IHubContext<ResourceNotificationHub> _hubContext = hubContext;
     
     public async Task NotifyResourceChangeToGroup(string resourceName, ResourceOperation operation, string id)
     {
          Console.WriteLine("Sending notification for {0} {1} {2}", resourceName, operation, id);
        
          var notification = new ResourceNotification(resourceName, operation, id);
          await _hubContext.Clients.Group(resourceName).SendAsync("notification", notification);
     }
}
