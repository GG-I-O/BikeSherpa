using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Services.Hateoas;

public interface IHateoasService
{
     string GenerateLink(string routeName, object routeValues);
     List<Link> GenerateLinks(string? getRouteName, string? putRouteName, string? deleteRouteName, object routeValues);
}
