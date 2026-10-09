using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Delivery;

public record GetReportQuery(
     Guid DeliveryId
) : IQuery<Report>;

public class GetReportQueryValidator : AbstractValidator<GetReportQuery>
{
     public GetReportQueryValidator(IReadRepository<Domain.DeliveryAggregate.Delivery> deliveryRepository)
     {
          RuleFor(x => x.DeliveryId)
               .NotEmpty()
               .MustAsync(async (deliveryId, cancellationToken) =>
                    await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(deliveryId), cancellationToken) is not null)
               .WithMessage("Delivery does not exist");
     }
}

public class GetReportHandler(
     IReadRepository<Domain.DeliveryAggregate.Delivery> deliveryRepository,
     IReadRepository<Domain.CustomerAggregate.Customer> customerRepository,
     IValidator<GetReportQuery> validator,
     IReportService reportService
     ) : IQueryHandler<GetReportQuery, Report>
{
     public async ValueTask<Report> Handle(GetReportQuery query, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(query, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(query.DeliveryId), cancellationToken);
          
          var customer = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(delivery!.CustomerId), cancellationToken);

          return await reportService.GenerateDeliveryReportAsync(
               customer!.Name,
               delivery.StartDate,
               delivery.StartDate,
               [delivery]);
     }
}
