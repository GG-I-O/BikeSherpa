using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Update;

public class UpdateCourierEndpoint(IMediator mediator) : Endpoint<CourierCrud>
{
     public override void Configure()
     {
          Put("/courier/{courierId:guid}");
          Policies("write:couriers");
          Description(x => x.WithTags("courier"));
     }

     public override async Task HandleAsync(CourierCrud req, CancellationToken ct)
     {
          var command = new UpdateCourierCommand(
               Route<Guid>("courierId"),
               req.FirstName,
               req.LastName,
               req.Code,
               req.Email,
               req.PhoneNumber,
               req.Address
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}