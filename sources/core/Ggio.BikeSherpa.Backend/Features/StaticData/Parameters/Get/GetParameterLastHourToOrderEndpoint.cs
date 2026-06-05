using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.Parameters.Get;

public class GetParameterLastHourToOrderEndpoint(
     IParameterRepository parameterRepository
): EndpointWithoutRequest<int>
{
     public override void Configure()
     {
          Get("/general/parameters/lastHourToOrder");
          Description(x => x.WithTags("general"));
          AllowAnonymous();
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var lastHourToOrder = await parameterRepository.GetLastHourToOrderAsync();

          await Send.OkAsync(lastHourToOrder, ct);
     }
}
