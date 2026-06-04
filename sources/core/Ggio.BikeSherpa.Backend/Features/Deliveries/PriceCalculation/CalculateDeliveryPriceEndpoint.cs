using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.PriceCalculation;

public class CalculateDeliveryPriceEndpoint(IMediator mediator) : Endpoint<DeliveryCrud, CalculateDeliveryPriceResult>
{
     public override void Configure()
     {
          Post("/deliveries/price");
          AllowAnonymous();
          Description(x => x.WithTags("delivery").WithDescription("Calculate the price of a delivery"));
     }
     
     public override async Task HandleAsync(DeliveryCrud request, CancellationToken ct)
     {
          var query = new CalculateDeliveryPriceQuery(request);
          var result = await mediator.Send(query, ct);
          await Send.OkAsync(result, cancellation: ct);
     }
}
