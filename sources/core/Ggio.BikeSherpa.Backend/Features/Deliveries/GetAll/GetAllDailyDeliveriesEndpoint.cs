using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public class GetAllDailyDeliveriesEndpoint(
     IMediator mediator,
     IDeliveryLinks deliveryLinks,
     IDeliveryStepLinks deliveryStepLinks,
     UserContext userContext  
     ) : EndpointWithoutRequest<List<DeliveryDto>>
{
     public override void Configure()
     {
          Get("/deliveries/dailyDeliveries/{date}");
          Policies("read:myDeliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var userEmail = userContext.Email;
          
          var date = Route<string>("date", isRequired: true);
          if (date is null || !DateTimeOffset.TryParse(date, out _))
          {
               throw new ArgumentException("Invalid date format");
          }
          var query = new GetAllDailyDeliveriesQuery(
               UserEmail: userEmail,
               Date: DateTimeOffset.Parse(date)
          );
          
          var result = await mediator.Send(query, ct);
          if (result is GetAllDailyDeliveriesResult.Success success)
          {
               var deliveryDtoList = new List<DeliveryDto>();
               
               foreach (var delivery in success.Deliveries)
               {
                    var deliveryStepDtoList = delivery.Steps.Select(deliveryStep => new DeliveryStepDto
                    {
                         Data = deliveryStep.Data,
                         Links = deliveryStepLinks.GenerateLinks(delivery.Id, deliveryStep.Data.Id)
                    }).ToList();

                    delivery.Steps = deliveryStepDtoList;

                    var deliveryDto = new DeliveryDto
                    {
                         Data = delivery,
                         Links = deliveryLinks.GenerateLinks(delivery.Id)
                    };

                    deliveryDtoList.Add(deliveryDto);
               }
               await Send.OkAsync(deliveryDtoList, cancellation: ct);
               return;
          }

          if (result is GetAllDailyDeliveriesResult.CourierNotFound)
          {
               throw new UnauthorizedAccessException("User unauthorized");
          }
     }
}
