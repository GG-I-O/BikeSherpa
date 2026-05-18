using System.Security.Claims;
using Ggio.BikeSherpa.Backend.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Services.Middleware;

public class UserEmailClaimMiddleware(RequestDelegate next)
{
     public async Task InvokeAsync(HttpContext context, UserContext userContext)
     {
          if (context.User.Identity?.IsAuthenticated == true)
          {
               var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
               if (userEmail is null)
                    throw new UnauthorizedAccessException("User email claim is missing");

               userContext.Email = userEmail;
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
