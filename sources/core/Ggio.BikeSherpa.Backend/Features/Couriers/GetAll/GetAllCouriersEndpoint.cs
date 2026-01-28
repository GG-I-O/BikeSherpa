using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Ggio.BikeSherpa.Backend.Features.Couriers.Services;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.GetAll;

public class GetAllCouriersEndpoint(IMediator mediator, ICourierLinks courierLinks) : EndpointWithoutRequest<List<CourierDto>>
{
     public override void Configure()
     {
          Get("/couriers/{lastSync?}");
          Policies("read:couriers");
          Description(x => x.WithTags("courier"));
          
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var lastSync = Query<string?>("lastSync", isRequired: false);
          var query = new GetAllCouriersQuery(String.IsNullOrEmpty(lastSync) ? null : DateTimeOffset.Parse(lastSync));
          var couriers = await mediator.Send(query, ct);
          var courierDtoList = new List<CourierDto>();
          foreach (var courier in couriers)
          {
               var courierDto = new CourierDto
               {
                    Data = courier,
                    Links = courierLinks.GenerateLinks(courier.Id)
               };
               courierDtoList.Add(courierDto);
          }
          await Send.OkAsync(courierDtoList, ct);
     }
}
