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

          var links = hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetDeliveryEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateDeliveryEndpoint>() : null,
               canWrite ? IEndpoint.GetName<DeleteDeliveryEndpoint>() : null,
               new { deliveryId = id }
          );

          // PUT /delivery/{deliveryId}/pending
          var putPendingDeliveryRouteValues = new { deliveryId = id};
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<ValidateDeliveryEndpoint>(), putPendingDeliveryRouteValues),
                    Rel = "putDeliveryPending",
                    Method = "PUT" 
               });
          
          // PUT /delivery/{deliveryId}/renew
          var putRenewDeliveryRouteValues = new { deliveryId = id};
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<RenewDeliveryEndpoint>(), putRenewDeliveryRouteValues),
                    Rel = "putDeliveryRenew",
                    Method = "PUT" 
               });

          return links;
     }
}
