using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public class UpdateCustomerEndpoint(IMediator mediator) : Endpoint<Model.CustomerCrud>
{
     public override void Configure()
     {
          Put("/customer/{customerId:guid}");
          Policies("write:customers");
          Description(x => x.WithTags("customer"));
     }

     public override async Task HandleAsync(Model.CustomerCrud req, CancellationToken ct)
     {
          var command = new UpdateClientCommand(
               Route<Guid>("customerId"),
               req.Name,
               req.Code,
               req.Siret,
               req.Email,
               req.PhoneNumber,
               req.Address
          );

          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
          {
               await Send.OkAsync(new object(), ct);
          }
     }
}
