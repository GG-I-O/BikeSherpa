using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Services;
using Mediator;
using Microsoft.AspNetCore.Builder;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Get;

public class GetEndpoint(IMediator mediator, HateoasService hateoasService): EndpointWithoutRequest<CustomerDto>
{
     public override void Configure()
     {
          Get("/api/clients/{Id:guid}");
          Claims("scope", "read:customers");
          Description(x => x.WithName("GetCustomer"));
          Options(x => x.WithName("GetCustomer"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var customer = await mediator.Send(new GetClientQuery(Query<Guid>("Id")), ct);
          if (customer is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }
          
          var clientDto = new CustomerDto
          {
               Data = customer
          };
          clientDto.Links = hateoasService.GenerateLinks(
               IEndpoint.GetName<GetEndpoint>(),
               IEndpoint.GetName<AddClientEndpoint>(),
               IEndpoint.GetName<UpdateEndpoint>(),
               null,
               new { clientDto.Data.Id }
          );
          
          await Send.OkAsync(clientDto, ct);
     }
}
