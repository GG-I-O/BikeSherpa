using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Validators;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepsCommand(
     Guid DeliveryId,
     DeliveryStepCrud[] Steps
) : ICommand<Result<Guid>>;

[UsedImplicitly]
public class AddDeliveryStepsCommandValidator : AbstractValidator<AddDeliveryStepsCommand>
{
     public AddDeliveryStepsCommandValidator(IPackingSizeRepository packingSizeRepository)
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.Steps).NotEmpty();
          RuleForEach(x => x.Steps).ChildRules(step =>
          {
               step.RuleFor(x => x.StepType).IsInEnum();
               step.RuleFor(x => x.StepAddress).NotEmpty();
               step.RuleFor(x => x.PackingSize).SetValidator(new PackingSizeValidator(packingSizeRepository));
          });
     }
}

public class AddDeliveryStepsHandler(
     IValidator<AddDeliveryStepsCommand> validator,
     IApplicationTransaction transaction,
     IReadRepository<Delivery> deliveryRepository,
     IDeliveryZoneRepository deliveryZones,
     IPricingStrategyService pricingStrategyService,
     IItinerarySpi itineraryService,
     IPackingSizeRepository packingSizeRepository
) : ICommandHandler<AddDeliveryStepsCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryStepsCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          if (delivery is null)
          {
               return Result<Guid>.NotFound();
          }

          foreach (var step in command.Steps)
          {
               await delivery.AddStepAsync(
                    step.StepType,
                    step.StepAddress,
                    step.Comment,
                    step.NotBilled,
                    packingSizeRepository.GetByName(step.PackingSize)!,
                    deliveryZones,
                    itineraryService
               );
          }
          delivery.TotalPrice = await pricingStrategyService.CalculateDeliveryPriceWithoutVat(delivery);

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
