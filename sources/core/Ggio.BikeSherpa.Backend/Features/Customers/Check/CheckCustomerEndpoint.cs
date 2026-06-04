using FastEndpoints;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Check;

public record CheckCustomerQuery
{
     [QueryParam]
     public required string Code { get; init; }
 
     [QueryParam]
     public required string Email { get; init; }
}

public record CheckCustomerResponse
{
     public DeliveryType? DefaultDeliveryType { get; init; }
     
     public required string CustomerName { get; init; }
}

public class CheckCustomerQueryValidator : AbstractValidator<CheckCustomerQuery>
{
     public CheckCustomerQueryValidator()
     {
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.Email).NotEmpty().EmailAddress();
     }
}

public class CheckCustomerEndpoint(IReadRepository<Customer> repository , IValidator<CheckCustomerQuery> validator) : Endpoint<CheckCustomerQuery, CheckCustomerResponse>
{
     public override void Configure()
     {
          AllowAnonymous();
          Post("/customers/check");
          Description(x => x.WithTags("customer").WithDescription("Check if a customer exists"));
     }

     public override async Task HandleAsync(CheckCustomerQuery req, CancellationToken ct)
     {
          await validator.ValidateAndThrowAsync(req, ct);
          
          var customer = await repository.FirstOrDefaultAsync(new CustomerByCodeAndEmailSpecification(req.Code.Trim(), req.Email.Trim()), ct);
          if (customer is null)
          {
               await Send.NotFoundAsync(ct);
               return;
          }

          await Send.OkAsync(new CheckCustomerResponse
          {
               CustomerName = customer.Name,
               DefaultDeliveryType = customer.DefaultDeliveryType
          }, ct);
     }
}
