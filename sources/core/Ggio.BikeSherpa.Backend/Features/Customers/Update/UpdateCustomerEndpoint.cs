using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public class UpdateCustomerEndpoint(IMediator mediator) : Endpoint<CustomerCrud>
{
     public override void Configure()
     {
          Put("/api/customer/{customerId:guid}");
          Policies("write:customers");
          // Description(x => x.WithName("UpdateCustomer"));
          // Options(x => x.WithName("UpdateCustomer"));
     }

     public override async Task HandleAsync(CustomerCrud req, CancellationToken ct)
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
