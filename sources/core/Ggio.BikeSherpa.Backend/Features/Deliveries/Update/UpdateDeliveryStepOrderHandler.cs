using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Step;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

[UsedImplicitly]
public record UpdateDeliveryStepOrderRequest(int Increment);

public record UpdateDeliveryStepOrderCommand(
     Guid DeliveryId,
     Guid StepId,
     int Increment
) : ICommand<Result>;

[UsedImplicitly]
public class UpdateDeliveryStepOrderCommandValidator : AbstractValidator<UpdateDeliveryStepOrderCommand>
{
     public UpdateDeliveryStepOrderCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
          RuleFor(x => x.Increment).NotEmpty();
     }
}

public class UpdateDeliveryStepOrderHandler(
     IReadRepository<Delivery> deliveryRepository,
     IValidator<UpdateDeliveryStepOrderCommand> validator,
     IDeliveryChangeTimeService service,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateDeliveryStepOrderCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryStepOrderCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null) return Result.NotFound();

          var step = delivery.Steps.FirstOrDefault(s => s.Id == command.StepId);
          if (step is null) return Result.NotFound();

          await service.ChangeOrder(
               step,
               command.Increment < 0 ? MoveDirection.Up : MoveDirection.Down,
               cancellationToken
               );
          
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
