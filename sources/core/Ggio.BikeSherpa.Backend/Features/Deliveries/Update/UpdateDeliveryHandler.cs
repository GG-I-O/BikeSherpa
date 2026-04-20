using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryCommand(
     Guid Id,
     PricingStrategy PricingStrategy,
     DeliveryStatus Status,
     string Code,
     Guid CustomerId,
     string Urgency,
     double? TotalPrice,
     double? Discount,
     string ReportId,
     List<DeliveryStep> Steps,
     string[] Details,
     string PackingSize,
     bool InsulatedBox,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
) : ICommand<Result>;

public class UpdateDeliveryCommandValidator : AbstractValidator<UpdateDeliveryCommand>
{
     public UpdateDeliveryCommandValidator(IUrgencyRepository urgencies, IPackingSizeRepository packingSizes)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.PricingStrategy).IsInEnum().NotEmpty();
          RuleFor(x => x.Status).IsInEnum();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency)
               .NotEmpty()
               .Must(urgency => urgencies.GetAll().Any(u => string.Equals(u.Name, urgency, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Valeur d'urgence saisie invalide.");
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.Discount).NotEmpty();
          RuleForEach(x => x.Steps)
               .ChildRules(step =>
               {
                    step.RuleFor(s => s.StepAddress).NotNull();
                    step.RuleFor(s => s.StepType).IsInEnum().WithMessage("Type d'étape invalide.");
                    step.RuleFor(s => s.EstimatedDeliveryDate).NotEmpty();
               });
          RuleFor(x => x.PackingSize).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.PackingSize).NotEmpty().Must(packingSize => packingSizes.GetAll().Any(p => string.Equals(p.Name, packingSize, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Taille de colis saisie invalide.");
          RuleFor(x => x.PackingSize).NotEmpty();
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
     }
}

public class UpdateDeliveryHandler(
     IReadRepository<Delivery> repository,
     IValidator<UpdateDeliveryCommand> validator,
     IApplicationTransaction transaction,
     IDeliveryZoneRepository deliveryZones,
     IPricingStrategyService pricingStrategyService,
     IItinerarySpi itineraryService
) : ICommandHandler<UpdateDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var entity = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          entity.PricingStrategy = command.PricingStrategy;
          entity.Status = command.Status;
          entity.Code = command.Code;
          entity.CustomerId = command.CustomerId;
          entity.Urgency = command.Urgency;
          entity.TotalPrice = command.TotalPrice;
          entity.Discount = command.Discount;
          entity.ReportId = command.ReportId;
          entity.Details = command.Details;
          entity.PackingSize = command.PackingSize;
          entity.InsulatedBox = command.InsulatedBox;
          entity.ContractDate = command.ContractDate;
          entity.StartDate = command.StartDate;

          await entity.UpdateStepsAsync(command.Steps, deliveryZones, pricingStrategyService, itineraryService);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
