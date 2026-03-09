using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
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
     public UpdateDeliveryCommandValidator(IReadRepository<Delivery> repository, IUrgencyRepository urgencies, IPackingSizeRepository packingSizes)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.PricingStrategy).IsInEnum().NotEmpty();
          RuleFor(x => x.Status).IsInEnum().NotEmpty();
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
               .NotEmpty()
               .Must(urgency => urgencies.GetAll().Any(u => string.Equals(u.Name, urgency, StringComparison.OrdinalIgnoreCase)))
               .WithMessage("Valeur d'urgence saisie invalide.");
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.Discount).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty().CustomAsync(async (reportId, context, cancellationToken) =>
          {
               var reportIdIsValid = !await repository.AnyAsync(new DeliveryByReportIdSpecification(reportId), cancellationToken);
               if (!reportIdIsValid)
               {
                    context.AddFailure("Numéro de rapport déjà utilisé");
               }
          });
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
     IPricingStrategyService pricingStrategyService
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

          entity.UpdateSteps(command.Steps, deliveryZones, pricingStrategyService);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
