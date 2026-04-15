using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.StaticData.Urgencies.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.Urgencies.GetAll;

public class GetAllUrgenciesEndpoint(IMediator mediator) : EndpointWithoutRequest<List<UrgencyDto>>
{
     public override void Configure()
     {
          Get("/public/urgencies");
          Policies("read:deliveries");
          Description(x => x.WithTags("public"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = new GetAllUrgenciesQuery();
          var urgencies = await mediator.Send(query, ct);
          await Send.OkAsync(urgencies, ct);
     }
}
