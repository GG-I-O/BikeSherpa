using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Get;

public class GetCustomerEndpoint(IMediator mediator, ICustomerLinks customerLinks): EndpointWithoutRequest<CustomerDto>
{
     public override void Configure()
     {
          Get("/api/customer/{customerId:guid}");
          Claims("scope", "read:customers");
          // Description(x => x.WithName("GetCustomer"));
          // Options(x => x.WithName("GetCustomer"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var customer = await mediator.Send(new GetClientQuery(Query<Guid>("customerId")), ct);
          if (customer is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }
          
          var clientDto = new CustomerDto
          {
               Data = customer
          };
          clientDto.Links = customerLinks.GenerateLinks(clientDto.Data.Id);
          
          await Send.OkAsync(clientDto, ct);
     }
}
