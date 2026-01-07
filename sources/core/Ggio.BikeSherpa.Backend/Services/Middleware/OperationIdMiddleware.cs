using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Services.Middleware;

public class OperationIdMiddleware(RequestDelegate next, ILogger<OperationIdMiddleware> logger)
{
     public async Task InvokeAsync(HttpContext context)
     {
          var operationId = context.Request.Headers["operationId"].FirstOrDefault();

          context.Response.Headers["operationId"] = operationId;
          context.Items["OperationId"] = operationId;

          logger.BeginScope("OperationId: {0}", operationId);

          await next(context);
     }
}

public static class OperationIdExtensions
{
     extension(IApplicationBuilder builder)
     {
          public IApplicationBuilder UseOperationIdMiddleware()
          {
               return builder.UseMiddleware<OperationIdMiddleware>();
          }
     }
}
