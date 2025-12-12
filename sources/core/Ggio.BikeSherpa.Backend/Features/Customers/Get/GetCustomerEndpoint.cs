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
          Policies("read:customers");
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var customer = await mediator.Send(new GetClientQuery(Route<Guid>("customerId")), ct);
          if (customer is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }
          var customerDto = new CustomerDto
          {
               Data = customer,
               Links = customerLinks.GenerateLinks(customer.Id)
          };
          await Send.OkAsync(customerDto, ct);
     }
}
