using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryStepOrderRequest(int Order);

public record UpdateDeliveryStepOrderCommand(
     Guid DeliveryId,
     Guid StepId,
     int Order
) : ICommand<Result>;

public class UpdateDeliveryStepOrderCommandValidator : AbstractValidator<UpdateDeliveryStepOrderCommand>
{
     public UpdateDeliveryStepOrderCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
          RuleFor(x => x.Order).NotEmpty();
     }
}

public class UpdateDeliveryStepOrderhandler(
     IReadRepository<Delivery> deliveryRepository,
     IValidator<UpdateDeliveryStepOrderCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateDeliveryStepOrderCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryStepOrderCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          if (delivery is null)
          {
               return Result.NotFound();
          }

          delivery.UpdateStepOrder(command.StepId, command.Order);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
