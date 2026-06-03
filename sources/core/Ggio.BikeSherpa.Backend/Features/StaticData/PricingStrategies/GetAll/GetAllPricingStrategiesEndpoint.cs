using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.GetAll;

public class GetAllPricingStrategiesEndpoint(IMediator mediator): EndpointWithoutRequest<List<PricingStrategyDto>>
{
     public override void Configure()
     {
          Get("/general/pricingStrategies");
          Description(x => x.WithTags("general"));
          AllowAnonymous();
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = new GetAllPricingStrategiesQuery();
          var strategies = await mediator.Send(query, ct);
          await Send.OkAsync(strategies, ct);
     }
}
