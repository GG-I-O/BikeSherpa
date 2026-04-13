using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Public.PricingStrategies.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Public.PricingStrategies.GetAll;

public class GetAllPricingStrategiesEndpoint(IMediator mediator): EndpointWithoutRequest<List<PricingStrategyDto>>
{
     public override void Configure()
     {
          Get("/public/pricingStrategies");
          AllowAnonymous(Http.GET);
          Description(x => x.WithTags("public"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = new GetAllPricingStrategiesQuery();
          var strategies = await mediator.Send(query, ct);
          await Send.OkAsync(strategies, ct);
     }
}
