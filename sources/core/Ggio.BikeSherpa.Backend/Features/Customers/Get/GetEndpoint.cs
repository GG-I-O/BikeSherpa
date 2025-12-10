using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Services;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Mediator;
using Microsoft.AspNetCore.Builder;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Get;

public class GetEndpoint(IMediator mediator, IHateoasService hateoasService): EndpointWithoutRequest<CustomerDto>
{
     public override void Configure()
     {
          Get("/api/customer/{customerId:guid}");
          Claims("scope", "read:customers");
          Description(x => x.WithName("GetCustomer"));
          Options(x => x.WithName("GetCustomer"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var customer = await mediator.Send(new GetClientQuery(Query<Guid>("customerId")), ct);
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
