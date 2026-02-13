using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Services.Catalogs;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryCommand(
     Guid Id,
     PricingStrategyEnum PricingStrategyEnum,
     DeliveryStatusEnum StatusEnum,
     string Code,
     Guid CustomerId,
     string Urgency,
     double TotalPrice,
     Guid ReportId,
     List<DeliveryStep> Steps,
     string[] Details,
     double TotalWeight,
     int HighestPackageLength,
     string PackingSize,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
) : ICommand<Result>;

public class UpdateDeliveryCommandValidator : AbstractValidator<UpdateDeliveryCommand>
{
     public UpdateDeliveryCommandValidator(IReadRepository<Delivery> repository, IUrgencyCatalog urgencies)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.PricingStrategyEnum).NotEmpty();
          RuleFor(x => x.StatusEnum).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency)
               .NotEmpty()
               .Must(urgency => urgencies.Urgencies.Any(u => string.Equals(u.Name, urgency, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Valeur d'urgence saisie invalide.");
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleForEach(x => x.Steps)
               .ChildRules(step =>
               {
                    step.RuleFor(s => s.StepAddress).NotNull();
                    step.RuleFor(s => s.StepType).IsInEnum().WithMessage("Type d'étape invalide.");
                    step.RuleFor(s => s.EstimatedDeliveryDate).NotEmpty();
               });
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.TotalWeight).NotEmpty();
          RuleFor(x => x.HighestPackageLength).NotEmpty();
          RuleFor(x => x.PackingSize).NotEmpty();
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
     }
}

public class UpdateDeliveryHandler(
     IReadRepository<Delivery> repository,
     IValidator<UpdateDeliveryCommand> validator,
     IApplicationTransaction transaction, IDeliveryZoneCatalog deliveryZones
) : ICommandHandler<UpdateDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var entity = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          entity.PricingStrategy = command.PricingStrategyEnum;
          entity.Status = command.StatusEnum;
          entity.Code = command.Code;
          entity.CustomerId = command.CustomerId;
          entity.Urgency = command.Urgency;
          entity.TotalPrice = command.TotalPrice;
          entity.ReportId = command.ReportId;
          entity.Details = command.Details;
          entity.TotalWeight = command.TotalWeight;
          entity.HighestPackageLength = command.HighestPackageLength;
          entity.PackingSize = command.PackingSize;
          entity.ContractDate = command.ContractDate;
          entity.StartDate = command.StartDate;

          foreach (var step in command.Steps)
          {
               var zone = deliveryZones.FromAddress(step.StepAddress.City);

               if (step.Id == Guid.Empty)
               {
                    entity.AddStep(step.StepType, step.StepAddress, zone, step.Distance, step.EstimatedDeliveryDate);
               }
               else entity.UpdateStep(step.Id, step.StepType, step.Order, step.StepAddress, zone, step.Distance, step.EstimatedDeliveryDate);

          }

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}