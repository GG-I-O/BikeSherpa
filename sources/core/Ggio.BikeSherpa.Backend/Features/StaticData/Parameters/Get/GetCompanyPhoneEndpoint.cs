using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.Parameters.Get;

public class GetCompanyPhoneEndpoint
(
     IParameterRepository repository
) : EndpointWithoutRequest<string>
{
     public override void Configure()
     {
          Get("/general/parameters/companyPhone");
          Description(x => x.WithTags("general"));
          AllowAnonymous();
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var stackHolderInfo = await repository.GetStackHolderInfoAsync();

          await Send.OkAsync(stackHolderInfo.Phone, ct);
     }
}
