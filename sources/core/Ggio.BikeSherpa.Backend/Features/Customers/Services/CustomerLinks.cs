using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Services;

public class CustomerLinks(IHateoasService hateoasService): ICustomerLinks
{
     public List<Link> GenerateLinks(Guid id)
     {
          // Todo : Send endpoints names if the user have the right claims, send null if not
          
          return hateoasService.GenerateLinks(
               IEndpoint.GetName<GetCustomerEndpoint>(),
               IEndpoint.GetName<UpdateCustomerEndpoint>(),
               null,
               new { customerId = id }
          );
     }
}
