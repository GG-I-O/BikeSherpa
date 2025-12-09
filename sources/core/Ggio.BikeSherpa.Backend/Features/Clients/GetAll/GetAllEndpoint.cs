using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.GetAll;

public class GetAllEndpoint(IMediator mediator): EndpointWithoutRequest<List<ClientCrud>>
{
     public override void Configure()
     {
          Get("/api/clients");
          Claims("scope", "read:customers");
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var clients = await mediator.Send(new GetAllClientsQuery(), ct);
          await Send.OkAsync(clients, ct);
     }
}
