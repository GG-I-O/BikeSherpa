using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Add;

public class AddCustomerEndpoint(IMediator mediator, ICustomerLinks customerLinks) : Endpoint<CustomerCrud, AddResult<GuidWithHateoas>>
{
     public override void Configure()
     {
          Post("/api/customer");
          Claims("scope", "write:customers");
          // Description(x => x.WithName("AddCustomer"));
          // Options(x => x.WithName("AddCustomer"));
     }

     public override async Task HandleAsync(CustomerCrud req, CancellationToken ct)
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
          {
               var response = new GuidWithHateoas
               {
                    Id = result.Value,
                    Links = customerLinks.GenerateLinks(result.Value)
               };
               // await Send.CreatedAtAsync(new AddResult<GuidWithHateoas>(response), cancellation: ct);
               await Send.ResultAsync(TypedResults.Created("", response));
          }
     }
}
