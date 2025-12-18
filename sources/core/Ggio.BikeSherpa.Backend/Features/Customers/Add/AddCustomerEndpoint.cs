using FastEndpoints;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Add;

public class AddCustomerEndpoint(IMediator mediator) : Endpoint<Model.CustomerCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/customer");
          Policies("write:customers");
          Description(x => x.WithTags("customer").WithName("CreateCustomer"));
     }

     public override async Task HandleAsync(Model.CustomerCrud req, CancellationToken ct)
     {
          var command = new AddCustomerCommand(
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
               await Send.ResultAsync(TypedResults.Created("", new { Id = result.Value }));
          }
     }
}
