using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
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
     double? TotalPrice,
     double? Discount,
     Guid ReportId,
     string[] Details,
     string PackingSize,
     bool InsulatedBox,
     bool ExactTime,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
     ) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IReadRepository<Delivery> repository, IUrgencyRepository urgencies)
     {
          RuleFor(x => x.PricingStrategyEnum).IsInEnum().NotEmpty();
          RuleFor(x => x.StatusEnum).IsInEnum().NotEmpty();
          RuleFor(x => x.Code).NotEmpty().CustomAsync(async (code, context, cancellationToken) =>
          {
               var codeIsValid = !await repository.AnyAsync(new DeliveryByCodeSpecification(code), cancellationToken);
               if (!codeIsValid)
               {
                    context.AddFailure("Code de course déjà utilisé");
               }
          });
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency)
               .NotEmpty().
               Must(urgency => urgencies.Urgencies.Any(u => string.Equals(u.Name, urgency, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Valeur d'urgence saisie invalide.");
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.ExactTime).NotEmpty();
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
               command.PricingStrategyEnum,
               command.Code,
               command.CustomerId,
               command.Urgency,
               command.TotalPrice,
               command.Discount,
               command.ReportId,
               command.PackingSize,
               command.InsulatedBox,
               command.ExactTime,
               command.ContractDate,
               command.StartDate
               );

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
