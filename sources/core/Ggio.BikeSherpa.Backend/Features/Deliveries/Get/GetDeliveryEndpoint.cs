using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Get;

public class GetDeliveryEndpoint(IMediator mediator) : EndpointWithoutRequest<DeliveryCrud>
{
     public override void Configure()
     {
          Get("/api/delivery/{Id:guid}");
          Permissions("read:deliveries");
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var entity = await mediator.Send(new GetDeliveryQuery(Query<Guid>("Id")), ct);
          if (entity is null)
          {
               await Send.NotFoundAsync(ct);
          }

          await Send.OkAsync(entity!, ct);
     }
}
