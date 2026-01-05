using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Delete;

public class DeleteCustomerEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
         Delete("/customer");
         Policies("write:customers");
         Description(x => x.WithTags("customer"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new DeleteCustomerCommand(
               Route<Guid>("customerId"));
          
          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
          {
               await Send.OkAsync(new object(), ct);
          }
     }
}
