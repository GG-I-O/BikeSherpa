using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public class GetAllDeliveriesEndpoint(IMediator mediator) : EndpointWithoutRequest<List<DeliveryCrud>>
{
     public override void Configure()
     {
          Get("/api/deliveries");
          Claims("scope", "read:deliveries");
     } 

     public override async Task HandleAsync(CancellationToken ct)
     {
          var deliveries = await mediator.Send(new GetAllDeliveriesQuery(), ct);
          await Send.OkAsync(deliveries, cancellation: ct);
     }
}
