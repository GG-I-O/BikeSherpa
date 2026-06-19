using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Courier;

public record GetReportQuery(
     Guid CourierId,
     DateTimeOffset From,
     DateTimeOffset To
) : IQuery<Report>;


public class GetReportQueryValidator : AbstractValidator<GetReportQuery>
{
     public GetReportQueryValidator(IReadRepository<Domain.CourierAggregate.Courier> customerRepository)
     {
          RuleFor(x => x.CourierId)
               .NotEmpty()
               .MustAsync(async (customerId, cancellationToken) =>
                    await customerRepository.FirstOrDefaultAsync(new CourierByIdSpecification(customerId), cancellationToken) is not null)
               .WithMessage("Courier does not exist");

          RuleFor(x => x.From).NotEmpty();
          RuleFor(x => x.To).NotEmpty();
          RuleFor(x => x.From).LessThanOrEqualTo(x => x.To);
     }
}

public class GetReportHandler(
     IReadRepository<Delivery> repository,
     IReadRepository<Domain.CourierAggregate.Courier> customerRepository,
     IValidator<GetReportQuery> validator,
     IReportService service
) : IQueryHandler<GetReportQuery, Report>
{
     public async ValueTask<Report> Handle(GetReportQuery query, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(query, cancellationToken);

          var courier = await customerRepository.FirstOrDefaultAsync(new CourierByIdSpecification(query.CourierId), cancellationToken);

          var deliveries = await repository
               .ListAsync(
                    new DeliveryByCourierAndDateRangeSpecification(
                         query.CourierId,
                         query.From,
                         query.To
                    )
                    , cancellationToken
               );

          return await service.GenerateDeliveryReportAsync(
               courier!.GetFullName(),
               query.From,
               query.To,
               deliveries
          );
     }
}
