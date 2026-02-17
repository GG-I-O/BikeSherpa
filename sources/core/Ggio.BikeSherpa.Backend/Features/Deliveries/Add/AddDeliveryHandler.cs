using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Services.Catalogs;
using Ggio.BikeSherpa.Backend.Services.Repositories;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryCommand(
     PricingStrategyEnum PricingStrategyEnum,
     DeliveryStatusEnum StatusEnum,
     string Code,
     Guid CustomerId,
     string Urgency,
     double TotalPrice,
     Guid ReportId,
     string[] Details,
     string PackingSize,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
     ) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IReadRepository<Delivery> repository, IUrgencyRepository urgencies)
     {
          RuleFor(x => x.PricingStrategyEnum).IsInEnum().NotEmpty();
          RuleFor(x => x.StatusEnum).IsInEnum().NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency)
               .NotEmpty().
               Must(urgency => urgencies.Urgencies.Any(u => string.Equals(u.Name, urgency, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Valeur d'urgence saisie invalide.");
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.PackingSize).NotEmpty();
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
     }
}

public class AddDeliveryHandler(
     IDeliveryFactory factory,
     IValidator<AddDeliveryCommand> validator,
     IApplicationTransaction transaction,
     IPackingSizeRepository packingSizes,
     IUrgencyRepository urgencies,
     IDeliveryZoneRepository deliveryZones) : ICommandHandler<AddDeliveryCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await factory.CreateDeliveryAsync(
               command.PricingStrategyEnum,
               command.Code,
               command.CustomerId,
               command.Urgency,
               command.ReportId,
               command.PackingSize,
               command.ContractDate,
               command.StartDate
               );

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
