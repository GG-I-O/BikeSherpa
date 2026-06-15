using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Validators;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepCommand(
     Guid DeliveryId,
     StepType StepType,
     Address StepAddress,
     string? Comment,
     bool NotBilled,
     string PackingSize
) : ICommand<Result<Guid>>;

[UsedImplicitly]
public class AddDeliveryStepCommandValidator : AbstractValidator<AddDeliveryStepCommand>
{
     public AddDeliveryStepCommandValidator(IPackingSizeRepository packingSizeRepository)
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepType).IsInEnum();
          RuleFor(x => x.StepAddress).NotEmpty();
          RuleFor(x => x.PackingSize).SetValidator(new PackingSizeValidator(packingSizeRepository));
     }
}

public class AddDeliveryStepHandler(
     IValidator<AddDeliveryStepCommand> validator,
     IApplicationTransaction transaction,
     IReadRepository<Delivery> deliveryRepository,
     IDeliveryZoneRepository deliveryZones,
     IPricingStrategyService pricingStrategyService,
     IItinerarySpi itineraryService,
     IPackingSizeRepository packingSizeRepository
) : ICommandHandler<AddDeliveryStepCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryStepCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          if (delivery is null)
          {
               return Result<Guid>.NotFound();
          }

          var deliveryStep = await delivery.AddStepAsync(command.StepType,
               command.StepAddress,
               command.Comment,
               command.NotBilled,
               packingSizeRepository.GetByName(command.PackingSize)!,
               deliveryZones,
               itineraryService,
               pricingStrategyService);
         
          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(deliveryStep.Id);
     }
}
