using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.GetAll;

public class GetAllCustomersEndpoint(IMediator mediator, ICustomerLinks customerLinks) : EndpointWithoutRequest<List<CustomerDto>>
{
     public override void Configure()
     {
          Get("/api/customers/{lastSync?}");
          Claims("scope", "read:customers");
          // Description(x => x.WithName("GetAllCustomers"));
          // Options(x => x.WithName("GetAllCustomers"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var lastSync = Query<string?>("lastSync");
          var query = new GetAllClientsQuery(lastSync is null ? null : DateTimeOffset.Parse(lastSync));
          var clients = await mediator.Send(query, ct);
          var clientDtoList = new List<CustomerDto>();
          foreach (var client in clients)
          {
               var clientDto = new CustomerDto
               {
                    Data = client
               };
               clientDto.Links = customerLinks.GenerateLinks(clientDto.Data.Id);
               clientDtoList.Add(clientDto);
          }

          await Send.OkAsync(clientDtoList, ct);
     }
}
