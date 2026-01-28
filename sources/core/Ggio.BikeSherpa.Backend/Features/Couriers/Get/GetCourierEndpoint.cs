using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Ggio.BikeSherpa.Backend.Features.Couriers.Services;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Get;

public class GetCourierEndpoint(IMediator mediator, ICourierLinks courierLinks): EndpointWithoutRequest<CourierDto>
{
     public override void Configure()
     {
          Get("/courier/{courierId:guid}");
          Policies("read:couriers");
          Description(x => x.WithTags("courier"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var courier = await mediator.Send(new GetCourierQuery(Route<Guid>("courierId")), ct);
          if (courier is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }
          var courierDto = new CourierDto
          {
               Data = courier,
               Links = courierLinks.GenerateLinks(courier.Id)
          };
          await Send.OkAsync(courierDto, ct);
     }
}