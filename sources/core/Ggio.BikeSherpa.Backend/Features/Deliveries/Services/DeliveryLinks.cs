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
          // Get Links depending permissions
          var context = httpContextAccessor.HttpContext;
          if (context is null)
               return [];
          var scopes = context.User.FindAll("scope")
               .SelectMany(c => c.Value.Split(' '))
               .Distinct()
               .ToList();

          var canRead = scopes.Contains("read:deliveries");
          var canWrite = scopes.Contains("write:deliveries");
          if (!canRead && !canWrite)
               return [];

          return hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetDeliveryEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateDeliveryEndpoint>() : null,
               canWrite ? IEndpoint.GetName<DeleteDeliveryEndpoint>() : null,
               new { deliveryId = id }
          );
     }
}
