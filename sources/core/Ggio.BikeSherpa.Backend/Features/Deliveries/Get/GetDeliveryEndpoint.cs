using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Get;

public class GetDeliveryEndpoint(IMediator mediator, IDeliveryLinks deliveryLinks) : EndpointWithoutRequest<DeliveryCrud>
{
     public override void Configure()
     {
          Get("/delivery/{deliveryId:guid}");
          Policies("read:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var delivery = await mediator.Send(new GetDeliveryQuery(Query<Guid>("deliveryId")), ct);
          if (delivery is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }
          var deliveryDto = new DeliveryDto
          {
               Data = delivery,
               Links = deliveryLinks.GenerateLinks(delivery.Id)
          };
          await Send.OkAsync(delivery!, ct);
     }
}
