using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepCommand(
     Guid DeliveryId,
     StepType StepType,
     Address StepAddress,
     double Distance,
     DateTimeOffset EstimatedDeliveryDate
     ) : ICommand<Result<Guid>>;

[UsedImplicitly]
public class AddDeliveryStepCommandValidator : AbstractValidator<AddDeliveryStepCommand>
{
     public AddDeliveryStepCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepType).NotEmpty();
          RuleFor(x => x.StepAddress).NotEmpty();
          RuleFor(x => x.Distance).NotEmpty();
          RuleFor(x => x.EstimatedDeliveryDate).NotEmpty();
     }
}

public class AddDeliveryStepHandler(
     IValidator<AddDeliveryStepCommand> validator,
     IApplicationTransaction transaction,
     IReadRepository<Delivery> deliveryRepository,
     IDeliveryZoneRepository deliveryZones,
     IPricingStrategyService pricingStrategyService
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

          var deliveryStep = delivery.AddStep(command.StepType, command.StepAddress, command.Distance, command.EstimatedDeliveryDate, deliveryZones, pricingStrategyService);

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(deliveryStep.Id);
     }
}
