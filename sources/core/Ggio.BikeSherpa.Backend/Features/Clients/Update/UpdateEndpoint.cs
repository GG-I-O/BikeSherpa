using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.Update;

public class UpdateEndpoint(IMediator mediator) : Endpoint<ClientCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Put("/api/client/{Id:guid}");
          Claims("scope", "write:customers");
     }

     public override async Task HandleAsync(ClientCrud req, CancellationToken ct)
     {
          var command = new UpdateClientCommand(
               req.Id,
               req.Name,
               req.Code,
               req.Siret,
               req.Email,
               req.PhoneNumber,
               req.Address
          );

          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
               await Send.OkAsync(ct);
     }
}
