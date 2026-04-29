using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

[UsedImplicitly]
public record UpdateDeliveryStepTimeRequest(DateTimeOffset Date);

public record UpdateDeliveryStepTimeCommand(
     Guid DeliveryId,
     Guid StepId,
     DateTimeOffset Date
) : ICommand<Result>;

[UsedImplicitly]
public class UpdateDeliveryStepTimeCommandValidator : AbstractValidator<UpdateDeliveryStepTimeCommand>
{
     public UpdateDeliveryStepTimeCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
          RuleFor(x => x.Date).NotEmpty();
     }
}

public class UpdateDeliveryStepTimeHandler(
     IReadRepository<Delivery> deliveryRepository,
     IValidator<UpdateDeliveryStepTimeCommand> validator,
     IDeliveryChangeTimeService service
     ) : ICommandHandler<UpdateDeliveryStepTimeCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryStepTimeCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null) return Result.NotFound();

          var step = delivery.Steps.FirstOrDefault(s => s.Id == command.StepId);
          if (step is null) return Result.NotFound();

          await service.ChangeTime(step, command.Date, cancellationToken);
          
          return Result.Success();
     }
}
