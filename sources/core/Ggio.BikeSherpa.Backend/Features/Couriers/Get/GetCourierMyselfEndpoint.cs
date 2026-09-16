using System.Security.Claims;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Ggio.BikeSherpa.Backend.Features.Couriers.Services;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Get;

public class GetCourierMyselfEndpoint(IMediator mediator, ICourierLinks courierLinks): EndpointWithoutRequest<CourierDto>
{
     public override void Configure()
     {
          Get("/courier/myself");
          Description(x => x.WithTags("courier"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
          if (userEmail is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }
          
          var courier = await mediator.Send(new GetCourierMyselfQuery(userEmail), ct);
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
