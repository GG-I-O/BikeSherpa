using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Services.Notification;

public class ResourceNotificationMiddleware(RequestDelegate next)
{
     public async Task InvokeAsync(HttpContext context, IResourceNotificationService notificationService)
     {
          var originalBodyStream = context.Response.Body;

          using var responseBody = new MemoryStream();
          context.Response.Body = responseBody;

          var method = context.Request.Method;
          var path = context.Request.Path.Value ?? "";

          await next(context);

          var statusCode = context.Response.StatusCode;
          if (statusCode is >= 200 and < 300)
               await TryNotifyResourceChange(method, path, responseBody, notificationService);

          responseBody.Seek(0, SeekOrigin.Begin);
          await responseBody.CopyToAsync(originalBodyStream);
     }

     private async Task TryNotifyResourceChange(string method, string path, MemoryStream body, IResourceNotificationService service)
     {
          try
          {
               var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
               if (segments.Length < 1) return;

               var resourceName = segments[0].ToLowerInvariant();

               ResourceOperation? operation = method.ToUpper() switch
               {
                    "POST" => ResourceOperation.Post,
                    "PUT" => ResourceOperation.Put,
                    "DELETE" => ResourceOperation.Delete,
                    _ => null
               };

               if (!operation.HasValue) return;

               switch (operation)
               {
                    case ResourceOperation.Post:
                         body.Seek(0, SeekOrigin.Begin);
                         using (var reader = new StreamReader(body, leaveOpen: true))
                         {
                              var content = await reader.ReadToEndAsync();
                              body.Seek(0, SeekOrigin.Begin);
                              var data = JsonSerializer.Deserialize<JsonElement>(content);
                              data.TryGetProperty("id", out var id);
                              await service.NotifyResourceChangeToGroup(
                                   resourceName,
                                   operation.Value,
                                   id.ToString()
                              );
                         }

                         break;

                    case ResourceOperation.Put:
                    case ResourceOperation.Delete:
                         if (segments.Length < 2) return;
                         var urlId = segments[1];
                         await service.NotifyResourceChangeToGroup(
                              resourceName,
                              operation.Value,
                              urlId
                         );

                         break;
                    default:
                         return;
               }
          }
          catch (Exception ex)
          {
               Console.WriteLine("Error while notifying resource change :", ex);
          }
     }
}

public static class ResourceNotificationExtensions
{
     public static IApplicationBuilder UseResourceNotifications(this IApplicationBuilder builder)
     {
          return builder.UseMiddleware<ResourceNotificationMiddleware>();
     }
}
