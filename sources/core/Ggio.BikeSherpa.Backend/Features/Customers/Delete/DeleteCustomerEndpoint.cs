using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Delete;

public class DeleteCustomerEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
         Delete("/customer/{customerId:guid}");
         Policies("write:customers");
         Description(x => x.WithTags("customer"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new DeleteCustomerCommand(
               Route<Guid>("customerId"));
          
          var result = await mediator.Send(command, ct);
          await Send.ToEndpointWithoutRequestResult(result, ct);
     }
}
