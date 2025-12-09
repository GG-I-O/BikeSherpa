using Microsoft.AspNetCore.SignalR;

namespace Ggio.BikeSherpa.Backend.Features.Notification;

public class ResourceNotificationHub: Hub
{
     public async Task BroadcastResourceChange(ResourceNotification notification)
     {
          await Clients.All.SendAsync("notification", notification);
     }
    
     public override async Task OnConnectedAsync()
     {
          await base.OnConnectedAsync();
     }

     public override async Task OnDisconnectedAsync(Exception? exception)
     {
          await base.OnDisconnectedAsync(exception);
     }
    
     public async Task SubscribeToDataType(string dataType)
     {
          await Groups.AddToGroupAsync(Context.ConnectionId, dataType);
          Console.WriteLine("Client {0} subscribed to {1}", Context.ConnectionId, dataType);
     }

     public async Task UnsubscribeFromDataType(string dataType)
     {
          await Groups.RemoveFromGroupAsync(Context.ConnectionId, dataType);
          Console.WriteLine("Client {0} unsubscribed from {1}", Context.ConnectionId, dataType);
     }
}
