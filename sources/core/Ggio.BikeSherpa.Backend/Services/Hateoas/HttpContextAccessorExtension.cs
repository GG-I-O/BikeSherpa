using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Services.Hateoas;

public static class HttpContextAccessorExtension
{
     public static (bool CanRead, bool CanWrite)? GetResourceScopes(
          this IHttpContextAccessor accessor, string readScope, string writeScope)
     {
          var context = accessor.HttpContext;
          if (context is null) return null;

          var scopes = context.User.FindAll("scope")
               .SelectMany(c => c.Value.Split(' '))
               .Distinct()
               .ToList();

          var canRead = scopes.Contains(readScope);
          var canWrite = scopes.Contains(writeScope);
          return canRead || canWrite ? (CanRead: canRead, CanWrite: canWrite) : null;
     }
}
