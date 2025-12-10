using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public class UpdateCustomerEndpoint(IMediator mediator, ICustomerLinks customerLinks) : Endpoint<CustomerCrud, AddResult<GuidWithHateoas>>
{
     public override void Configure()
     {
          Put("/api/client/{customerId:guid}");
          Claims("scope", "write:customers");
          // Description(x => x.WithName("UpdateCustomer"));
          // Options(x => x.WithName("UpdateCustomer"));
     }

     public override async Task HandleAsync(CustomerCrud req, CancellationToken ct)
     {
          var command = new UpdateClientCommand(
               Query<Guid>("customerId"),
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
               await Send.OkAsync(new AddResult<GuidWithHateoas>(response), ct);
          }
     }
}
