using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services;
using Mediator;
using Microsoft.AspNetCore.Builder;

namespace Ggio.BikeSherpa.Backend.Features.Customers.GetAll;

public class GetAllEndpoint(IMediator mediator, HateoasService hateoasService) : EndpointWithoutRequest<List<CustomerDto>>
{
     public override void Configure()
     {
          Get("/api/customers");
          Claims("scope", "read:customers");
          Description(x => x.WithName("GetAllCustomers"));
          Options(x => x.WithName("GetAllCustomers"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var clients = await mediator.Send(new GetAllClientsQuery(Query<DateTimeOffset>("lastSync", isRequired: false)), ct);
          var clientDtoList = new List<CustomerDto>();
          foreach (var client in clients)
          {
               var clientDto = new CustomerDto
               {
                    Data = client
               };
               clientDto.Links = hateoasService.GenerateLinks(
                    IEndpoint.GetName<GetEndpoint>(),
                    IEndpoint.GetName<AddClientEndpoint>(),
                    IEndpoint.GetName<UpdateEndpoint>(),
                    null,
                    new { clientDto.Data.Id }
               );
               clientDtoList.Add(clientDto);
          }

          await Send.OkAsync(clientDtoList, ct);
     }
}
