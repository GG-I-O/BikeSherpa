using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public class GetAllDeliveriesEndpoint(IMediator mediator, IDeliveryLinks deliveryLinks) : EndpointWithoutRequest<List<DeliveryDto>>
{
     public override void Configure()
     {
          Get("/deliveries/{lastSync?}");
          Policies("read:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var lastSync = Query<string?>("lastSync", isRequired: false);
          var query = new GetAllDeliveriesQuery(String.IsNullOrEmpty(lastSync) ? null : DateTimeOffset.Parse(lastSync));
          var deliveries = await mediator.Send(query, ct);
          var deliveryDtoList = new List<DeliveryDto>();
          foreach (var delivery in deliveries)
          {
               var deliveryDto = new DeliveryDto
               {
                    Data = delivery,
                    Links = deliveryLinks.GenerateLinks(delivery.Id)
               };
               deliveryDtoList.Add(deliveryDto);
          }
          await Send.OkAsync(deliveryDtoList, ct);
     }
}
