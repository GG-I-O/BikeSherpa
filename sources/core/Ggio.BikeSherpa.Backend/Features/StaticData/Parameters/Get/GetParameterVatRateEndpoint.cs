using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.Parameters.Get;

public class GetParameterVatRateEndpoint(
     IParameterRepository parameterRepository
     ): EndpointWithoutRequest<double>
{
     public override void Configure()
     {
          Get("/general/parameters/vatRate");
          Description(x => x.WithTags("general"));
          AllowAnonymous();
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var vatRate = await parameterRepository.GetVatRateAsync();

          await Send.OkAsync(vatRate, ct);
     }
}
