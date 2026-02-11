using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Couriers.Get;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Add;

public class AddCourierEndpoint(IMediator mediator) : Endpoint<CourierCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/courier");
          Policies("write:couriers");
          Description(x => x.WithTags("courier"));
     }

     public override async Task HandleAsync(CourierCrud req, CancellationToken ct)
     {
          var command = new AddCourierCommand(
               req.FirstName,
               req.LastName,
               req.Code,
               req.Email,
               req.PhoneNumber,
               req.Address
          );

          var result = await mediator.Send(command, ct);

          await Send.CreatedAtAsync<GetCourierEndpoint>(result.Value, new AddResult<Guid>(result.Value), cancellation: ct);
     }
}