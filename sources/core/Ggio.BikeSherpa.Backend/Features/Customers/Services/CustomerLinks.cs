using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Services;

public class CustomerLinks(IHttpContextAccessor httpContextAccessor, IHateoasService hateoasService) : ICustomerLinks
{
     public List<Link> GenerateLinks(Guid id)
     {
          // Get Links depending on permissions
          var scopes = httpContextAccessor.GetResourceScopes("read:customers", "write:customers");

          if (scopes is null) return [];

          var (canRead, canWrite) = scopes.Value;

          return hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetCustomerEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateCustomerEndpoint>() : null,
               canWrite ? IEndpoint.GetName<DeleteCustomerEndpoint>() : null,
               new { customerId = id }
          );
     }
}
