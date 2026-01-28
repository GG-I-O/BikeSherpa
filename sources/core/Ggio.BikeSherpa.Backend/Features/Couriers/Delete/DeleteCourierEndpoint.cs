using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Delete;

public class DeleteCourierEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Delete("/courier/{courierId:guid}");
          Policies("write:couriers");
          Description(x => x.WithTags("courier"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new DeleteCourierCommand(
               Route<Guid>("courierId"));
          
          var result = await mediator.Send(command, ct);
          await Send.ToEndpointWithoutRequestResult(result, ct);
     }
}
