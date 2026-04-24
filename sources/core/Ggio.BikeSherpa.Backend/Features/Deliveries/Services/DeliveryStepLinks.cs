using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public class DeliveryStepLinks(IHttpContextAccessor httpContextAccessor, IHateoasService hateoasService): IDeliveryStepLinks
{
     public List<Link> GenerateLinks(Guid deliveryId, Guid stepId)
     {
          // Get Links depending on permissions
          var scopes = httpContextAccessor.GetResourceScopes("read:deliveries", "write:deliveries");

          if (scopes is null) return [];

          var (canRead, canWrite) = scopes.Value;
          
          var links = new List<Link>();
          var routeValues = new { deliveryId, stepId };
          
          // PATCH /delivery/{deliveryId}/step/{stepId}/time
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<PatchDeliveryStepTimeEndpoint>(), routeValues),
                    Rel = "patchTime",
                    Method = "PATCH" 
               });
          
          // PATCH /delivery/{deliveryId}/step/{stepId}/order
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<PatchDeliveryStepOrderEndpoint>(), routeValues),
                    Rel = "patchOrder",
                    Method = "PATCH" 
               });
          
          // PUT /delivery/{deliveryId}/step/{stepId}/complete
          
          // POST /delivery{deliveryId}/step/{stepId}/courier/{ID}
          
          // Delete /delivery{deliveryId}/step/{stepId}/courier/{ID}

          return links;
     }
}
