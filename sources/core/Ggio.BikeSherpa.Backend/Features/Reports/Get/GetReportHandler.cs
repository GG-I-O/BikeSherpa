using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Get;

public record GetReportQuery(
     Guid CustomerId,
     DateTimeOffset From,
     DateTimeOffset To
) : IQuery<Report>;

public class GetReportQueryValidator : AbstractValidator<GetReportQuery>
{
     public GetReportQueryValidator(IReadRepository<Customer> customerRepository)
     {
          RuleFor(x => x.CustomerId)
               .NotEmpty()
               .MustAsync(async (customerId, cancellationToken) =>
                    await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(customerId), cancellationToken) is not null)
               .WithMessage("Customer does not exist");

          RuleFor(x => x.From).NotEmpty();
          RuleFor(x => x.To).NotEmpty();
          RuleFor(x => x.From).LessThanOrEqualTo(x => x.To);
     }
}

public class GetReportHandler(
     IReadRepository<Delivery> repository,
     IReadRepository<Customer> customerRepository,
     IValidator<GetReportQuery> validator,
     IReportService service
) : IQueryHandler<GetReportQuery, Report>
{
     public async ValueTask<Report> Handle(GetReportQuery query, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(query, cancellationToken);
          
          var customer = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(query.CustomerId), cancellationToken);

          var deliveries = await repository
               .ListAsync(
                    new DeliveryByCustomerAndDateRangeSpecification(
                         query.CustomerId,
                         query.From,
                         query.To
                    )
                    , cancellationToken
               );

          return await service.GenerateReport(
               customer!.Name,
               query.From,
               query.To,
               deliveries
          );
     }
}
