using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
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

          var (_, canWrite) = scopes.Value;
          
          var links = new List<Link>();
          
          // PATCH /delivery/{deliveryId}/step/{stepId}
          var routeValues = new { deliveryId, stepId };
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<PatchDeliveryStepEndpoint>(), routeValues),
                    Rel = "patch",
                    Method = "PATCH" 
               });
          
          // PUT /delivery/{deliveryId}/step/{stepId}/complete
          
          // POST /delivery{deliveryId}/step/{stepId}/courier/{ID}
          var postCourierRouteValues = new { deliveryId, stepId, courierId = Guid.Empty};
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<AddDeliveryStepCourierEndpoint>(), postCourierRouteValues),
                    Rel = "postCourier",
                    Method = "POST" 
               });
          
          // Delete /delivery{deliveryId}/step/{stepId}/courier
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<DeleteDeliveryStepCourierEndpoint>(), routeValues),
                    Rel = "deleteCourier",
                    Method = "DELETE" 
               });

          // PUT /delivery/{deliveryId}/step/{stepId}/changeOrder
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<UpdateDeliveryStepOrderEndpoint>(), routeValues),
                    Rel = "putOrder",
                    Method = "PUT" 
               });
          
          // PUT /delivery/{deliveryId}/step/{stepId}/changeTime
          if (canWrite)
               links.Add(new Link {
                    Href = hateoasService.GenerateLink(IEndpoint.GetName<UpdateDeliveryStepTimeEndpoint>(), routeValues),
                    Rel = "putTime",
                    Method = "PUT" 
               });
          
          return links;
     }
}
