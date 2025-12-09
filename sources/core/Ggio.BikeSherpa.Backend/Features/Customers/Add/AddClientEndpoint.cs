using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services;
using Mediator;
using Microsoft.AspNetCore.Builder;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Add;

public class AddClientEndpoint(IMediator mediator, HateoasService hateoasService) : Endpoint<CustomerCrud, AddResult<GuidWithHateoas>>
{
     public override void Configure()
     {
          Post("/api/client");
          Claims("scope", "write:customers");
          Description(x => x.WithName("AddCustomer"));
          Options(x => x.WithName("AddCustomer"));
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
                    Links = hateoasService.GenerateLinks(
                         IEndpoint.GetName<GetEndpoint>(),
                         IEndpoint.GetName<AddClientEndpoint>(),
                         IEndpoint.GetName<UpdateEndpoint>(),
                         null,
                         new { result.Value }
                    )
               };
               await Send.CreatedAtAsync<GetEndpoint>(new AddResult<GuidWithHateoas>(response), cancellation: ct);
          }
     }
}
