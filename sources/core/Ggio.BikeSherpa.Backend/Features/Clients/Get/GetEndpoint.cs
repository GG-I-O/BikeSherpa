using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.Get;

public class GetEndpoint(IMediator mediator): EndpointWithoutRequest<ClientCrud>
{
     public override void Configure()
     {
          Get("/api/client/{Id:guid}");
          Claims("scope", "read:customers");
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var entity = await mediator.Send(new GetClientQuery(Query<Guid>("Id")), ct);
          if (entity is null)
               await Send.NotFoundAsync(ct);
          await Send.OkAsync(entity!, ct);
     }
}
