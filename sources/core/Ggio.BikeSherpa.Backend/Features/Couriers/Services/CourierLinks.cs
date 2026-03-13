using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Couriers.Delete;
using Ggio.BikeSherpa.Backend.Features.Couriers.Get;
using Ggio.BikeSherpa.Backend.Features.Couriers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Services;

public class CourierLinks(IHttpContextAccessor httpContextAccessor, IHateoasService hateoasService) : ICourierLinks
{
     public List<Link> GenerateLinks(Guid id)
     {
          // Get Links depending on permissions
          var scopes = httpContextAccessor.GetResourceScopes("read:couriers", "write:couriers");

          if (scopes is null) return [];

          var (canRead, canWrite) = scopes.Value;

          return hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetCourierEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateCourierEndpoint>() : null,
               canWrite ? IEndpoint.GetName<DeleteCourierEndpoint>() : null,
               new { courierId = id }
          );
     }
}
