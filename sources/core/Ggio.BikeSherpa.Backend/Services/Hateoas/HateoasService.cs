using Ggio.BikeSherpa.Backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ggio.BikeSherpa.Backend.Services.Hateoas;

public class HateoasService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator): IHateoasService
{
     public string GenerateLink(string routeName, object? routeValues)
     {
          var httpContext = httpContextAccessor.HttpContext;
          if (httpContext == null) return "";
          return linkGenerator.GetUriByName(httpContext, routeName, routeValues) ?? string.Empty;
     }

     public List<Link> GenerateLinks(string? getRouteName, string? putRouteName, string? deleteRouteName, object routeValues)
     {
          var links = new List<Link>();
          
          if (string.IsNullOrEmpty(getRouteName)) return links;
          links.Add(new Link { Href = GenerateLink(getRouteName, routeValues), Rel = "self", Method = "GET" });
          
          if (string.IsNullOrEmpty(putRouteName)) return links;
          links.Add(new Link { Href = GenerateLink(putRouteName, routeValues), Rel = "update", Method = "PUT" });
          
          if (string.IsNullOrEmpty(deleteRouteName)) return links;
          links.Add(new Link { Href = GenerateLink(deleteRouteName, routeValues), Rel = "delete", Method = "DELETE" });

          return links;
     }
}
