using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Validators;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryCommand(
     PricingStrategy PricingStrategy,
     Guid CustomerId,
     string Urgency,
     double? TotalPrice,
     double? Discount,
     double? ExtraCost,
     string[] Details,
     bool InsulatedBox,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate,
     bool NeedEstimate,
     string? DiscountReason,
     string? ExtraCostReason
) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IUrgencyRepository urgencies)
     {
          RuleFor(x => x.PricingStrategy).IsInEnum();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency).SetValidator(new UrgencyValidator(urgencies));
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
     }
}

public class AddDeliveryHandler(
     IDeliveryFactory factory,
     IUrgencyRepository urgencyRepository,
     IValidator<AddDeliveryCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<AddDeliveryCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var urgency = urgencyRepository.GetByName(command.Urgency)!;

          var delivery = await factory.CreateDeliveryAsync(new DeliveryFactoryParameters(
                    command.PricingStrategy,
                    command.CustomerId,
                    urgency,
                    command.TotalPrice,
                    command.Discount,
                    command.ExtraCost,
                    command.Details,
                    command.InsulatedBox,
                    command.ContractDate,
                    command.StartDate,
                    command.NeedEstimate,
                    command.DiscountReason,
                    command.ExtraCostReason
               )
          );

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
