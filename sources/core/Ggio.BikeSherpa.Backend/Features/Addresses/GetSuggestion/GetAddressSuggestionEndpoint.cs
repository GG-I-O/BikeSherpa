using FastEndpoints;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Addresses.GetSuggestion;

public class GetAddressSuggestionEndpoint(IMediator mediator): EndpointWithoutRequest<List<AddressCrud>>
{
     public override void Configure()
     {
          Get("/address/suggest/{query:required}");
          Description(x => x.WithTags("address"));
          AllowAnonymous();
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = Route<string>("query");
          if (query is null)
          {
               await Send.NotFoundAsync(ct);
               return; 
          }

          var addresses = await mediator.Send(new GetAddressSuggestionQuery(query), ct);

          await Send.OkAsync(addresses, ct);
     }
}
