using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Clients.Get;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.Add;

public class AddClientEndpoint(IMediator mediator) : Endpoint<ClientCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/api/client");
          Claims("scope", "write:customers");
     }

     public override async Task HandleAsync(ClientCrud req, CancellationToken ct)
     {
          var command = new AddClientCommand(
               req.Name,
               req.Code,
               req.Siret,
               req.Email,
               req.PhoneNumber,
               req.Address
          );

          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
               await Send.CreatedAtAsync<GetEndpoint>(new AddResult<Guid>(result.Value), cancellation: ct);
     }
}
