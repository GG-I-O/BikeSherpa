using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryCommand(
     PricingStrategy PricingStrategy,
     Guid CustomerId,
     string Urgency,
     double? TotalPrice,
     double? Discount,
     string[] Details,
     string PackingSize,
     bool InsulatedBox,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
     ) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IUrgencyRepository urgencies, IPackingSizeRepository packingSizes)
     {
          RuleFor(x => x.PricingStrategy).IsInEnum().NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency)
               .NotEmpty().
               Must(urgency => urgencies.GetAll().Any(u => string.Equals(u.Name, urgency, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Valeur d'urgence saisie invalide.");
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.PackingSize).NotEmpty().Must(packingSize => packingSizes.GetAll().Any(p => string.Equals(p.Name, packingSize, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Taille de colis saisie invalide.");
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
     }
}

public class AddDeliveryHandler(
     IDeliveryFactory factory,
     IValidator<AddDeliveryCommand> validator,
     IApplicationTransaction transaction
     ) : ICommandHandler<AddDeliveryCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await factory.CreateDeliveryAsync(
               command.PricingStrategy,
               command.CustomerId,
               command.Urgency,
               command.TotalPrice,
               command.Discount,
               command.Details,
               command.PackingSize,
               command.InsulatedBox,
               command.ContractDate,
               command.StartDate
               );

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
