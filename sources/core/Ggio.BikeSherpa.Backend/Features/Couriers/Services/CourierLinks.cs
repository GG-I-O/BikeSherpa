using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Couriers.Delete;
using Ggio.BikeSherpa.Backend.Features.Couriers.Get;
using Ggio.BikeSherpa.Backend.Features.Couriers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Services;

public class CourierLinks(IHttpContextAccessor httpContextAccessor, IHateoasService hateoasService): ICourierLinks
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
          
          var canRead = scopes.Contains("read:couriers");
          var canWrite = scopes.Contains("write:couriers");
          if (!canRead && !canWrite)
               return [];

          return hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetCourierEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateCourierEndpoint>() : null,
               canWrite ? IEndpoint.GetName<DeleteCourierEndpoint>() : null,
               new { courierId = id }
          );
     }
}
