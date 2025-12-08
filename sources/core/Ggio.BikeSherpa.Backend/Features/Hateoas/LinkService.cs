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

     public List<Link> GenerateLinks(string? getRoute, string? postRoute, string? putRoute, string? deleteRoute, object routeValues)
     {
          var links = new List<Link>();
          
          if (string.IsNullOrEmpty(getRoute)) return links;
          links.Add(new Link { Href = GenerateLink(getRoute, routeValues), Rel = "self", Method = "GET" });
          
          if (string.IsNullOrEmpty(postRoute)) return links;
          links.Add(new Link { Href = GenerateLink(postRoute, routeValues), Rel = "create", Method = "POST" });
          
          if (string.IsNullOrEmpty(putRoute)) return links;
          links.Add(new Link { Href = GenerateLink(putRoute, routeValues), Rel = "update", Method = "PUT" });
          
          if (string.IsNullOrEmpty(deleteRoute)) return links;
          links.Add(new Link { Href = GenerateLink(deleteRoute, routeValues), Rel = "delete", Method = "DELETE" });

          return links;
     }
}
