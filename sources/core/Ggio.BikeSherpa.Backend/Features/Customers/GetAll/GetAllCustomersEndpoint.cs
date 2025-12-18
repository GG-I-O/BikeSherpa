using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.GetAll;

public class GetAllCustomersEndpoint(IMediator mediator, ICustomerLinks customerLinks) : EndpointWithoutRequest<List<CustomerDto>>
{
     public override void Configure()
     {
          Get("/customers/{lastSync?}");
          Policies("read:customers");
          Description(x => x.WithTags("customer").WithName("GetCustomers"));
          
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var lastSync = Query<string?>("lastSync", isRequired: false);
          var query = new GetAllCustomersQuery(String.IsNullOrEmpty(lastSync) ? null : DateTimeOffset.Parse(lastSync));
          var customers = await mediator.Send(query, ct);
          var customerDtoList = new List<CustomerDto>();
          foreach (var customer in customers)
          {
               var customerDto = new CustomerDto
               {
                    Data = customer,
                    Links = customerLinks.GenerateLinks(customer.Id)
               };
               customerDtoList.Add(customerDto);
          }
          await Send.OkAsync(customerDtoList, ct);
     }
}
