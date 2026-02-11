using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryCommand(
     PricingStrategyEnum PricingStrategyEnum,
     DeliveryStatusEnum StatusEnum,
     string Code,
     Guid CustomerId,
     double TotalPrice,
     Guid ReportId,
     string[] Details,
     double TotalWeight,
     int HighestLength,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
     ) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IReadRepository<Delivery> repository)
     {
          RuleFor(x => x.PricingStrategyEnum).NotEmpty();
          RuleFor(x => x.StatusEnum).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.TotalWeight).NotEmpty();
          RuleFor(x => x.HighestLength).NotEmpty();
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
     }
}

public class AddDeliveryHandler(
     IDeliveryFactory factory,
     IValidator<AddDeliveryCommand> validator,
     IApplicationTransaction transaction) : ICommandHandler<AddDeliveryCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          
          var delivery = await factory.CreateDeliveryAsync(
               command.PricingStrategyEnum,
               command.StatusEnum,
               command.Code,
               command.CustomerId,
               command.TotalPrice,
               command.ReportId,
               command.Details,
               command.TotalWeight,
               command.HighestLength,
               command.ContractDate,
               command.StartDate
               );
          
         await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
