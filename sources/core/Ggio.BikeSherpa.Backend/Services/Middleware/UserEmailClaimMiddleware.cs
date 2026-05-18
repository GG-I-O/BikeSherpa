using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Services.Middleware;

public class UserEmailClaimMiddleware(RequestDelegate next)
{
     public async Task InvokeAsync(HttpContext context)
     {
          if (context.User.Identity?.IsAuthenticated == true)
          {
               var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
               if (userEmail is null)
               {
                    throw new UnauthorizedAccessException("User email claim is missing");
               }
          }

          await next(context);
     }
}

public static class UserEmailClaimExtensions
{
     extension(IApplicationBuilder builder)
     {
          public IApplicationBuilder UseUserEmailClaimMiddleware() => builder.UseMiddleware<UserEmailClaimMiddleware>();
     }
}
