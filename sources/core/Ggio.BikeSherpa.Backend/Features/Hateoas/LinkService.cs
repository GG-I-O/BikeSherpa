using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ggio.BikeSherpa.Backend.Features.Hateoas;

public class LinkService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
{
     public string GenerateLink(string routeName, object routeValues)
     {
          var httpContext = httpContextAccessor.HttpContext;
          if (httpContext == null) return "";
          return linkGenerator.GetUriByName(httpContext, routeName, routeValues) ?? string.Empty;
     }
}
