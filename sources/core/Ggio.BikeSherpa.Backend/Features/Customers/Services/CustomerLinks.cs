using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Services;

public class CustomerLinks(IHttpContextAccessor httpContextAccessor, IHateoasService hateoasService): ICustomerLinks
{
     public List<Link> GenerateLinks(Guid id)
     {
          // Get Links depending permissions
          var context = httpContextAccessor.HttpContext;
          if (context is null)
               return new List<Link>();
          var scopes = context.User.FindAll("scope")
               .SelectMany(c => c.Value.Split(' '))
               .Distinct()
               .ToList();
          
          var canRead = scopes.Contains("read:customers");
          var canWrite = scopes.Contains("write:customers");

          return hateoasService.GenerateLinks(
               canRead ? IEndpoint.GetName<GetCustomerEndpoint>() : null,
               canWrite ? IEndpoint.GetName<UpdateCustomerEndpoint>() : null,
               canRead ? IEndpoint.GetName<DeleteCustomerEndpoint>() : null,
               new { customerId = id }
          );
     }
}
