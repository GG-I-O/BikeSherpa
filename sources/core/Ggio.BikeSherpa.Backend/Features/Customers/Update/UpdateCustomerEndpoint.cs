using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public class UpdateCustomerEndpoint(IMediator mediator) : Endpoint<CustomerCrud>
{
     public override void Configure()
     {
          Put("/customer/{customerId:guid}");
          Policies("write:customers");
          Description(x => x.WithTags("customer"));
     }

     public override async Task HandleAsync(CustomerCrud req, CancellationToken ct)
     {
          var command = new UpdateCustomerCommand(
               Route<Guid>("customerId"),
               req.Name,
               req.Code,
               req.Siret,
               req.Email,
               req.PhoneNumber,
               req.Address
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
