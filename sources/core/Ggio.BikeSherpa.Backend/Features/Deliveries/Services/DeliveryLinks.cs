using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public class DeliveryLinks(IHttpContextAccessor httpContextAccessor, IHateoasService hateoasService) : IDeliveryLinks
{
     public List<Link> GenerateLinks(Guid id)
     {
          // Get Links depending on permissions
          var scopes = httpContextAccessor.GetResourceScopes("read:deliveries", "write:deliveries");

          if (scopes is null) return [];

          var (canRead, canWrite) = scopes.Value;

          return hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetDeliveryEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateDeliveryEndpoint>() : null,
               canWrite ? IEndpoint.GetName<DeleteDeliveryEndpoint>() : null,
               new { deliveryId = id }
          );
     }
}
