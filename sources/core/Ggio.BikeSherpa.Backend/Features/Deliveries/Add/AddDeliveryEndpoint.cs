using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public class AddDeliveryEndpoint(IMediator mediator) : Endpoint<DeliveryCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/api/delivery");
          Permissions("write:deliveries");
     }

     public override async Task HandleAsync(DeliveryCrud req, CancellationToken ct)
     {
          var command = new AddDeliveryCommand(req.StartDate);
          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
          {
               await Send.CreatedAtAsync<GetDeliveryEndpoint>(new AddResult<Guid>(result.Value), cancellation: ct);
          }
         
     }
}
