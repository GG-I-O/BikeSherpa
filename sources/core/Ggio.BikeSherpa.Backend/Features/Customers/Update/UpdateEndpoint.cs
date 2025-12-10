using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Mediator;
using Microsoft.AspNetCore.Builder;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public class UpdateEndpoint(IMediator mediator, IHateoasService hateoasService) : Endpoint<CustomerCrud, AddResult<GuidWithHateoas>>
{
     public override void Configure()
     {
          Put("/api/client/{customerId:guid}");
          Claims("scope", "write:customers");
          Description(x => x.WithName("UpdateCustomer"));
          Options(x => x.WithName("UpdateCustomer"));
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
                    Links = hateoasService.GenerateLinks(
                         IEndpoint.GetName<GetEndpoint>(),
                         IEndpoint.GetName<AddClientEndpoint>(),
                         IEndpoint.GetName<UpdateEndpoint>(),
                         null,
                         new { result.Value }
                    )
               };
               await Send.OkAsync(new AddResult<GuidWithHateoas>(response), ct);
          }
     }
}
