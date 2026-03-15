using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

[UsedImplicitly]
public record UpdateDeliveryStepOrderRequest(int Order);

public record UpdateDeliveryStepOrderCommand(
     Guid DeliveryId,
     Guid StepId,
     int Order
) : ICommand<Result>;

[UsedImplicitly]
public class UpdateDeliveryStepOrderCommandValidator : AbstractValidator<UpdateDeliveryStepOrderCommand>
{
     public UpdateDeliveryStepOrderCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
          RuleFor(x => x.Order).NotEmpty();
     }
}

public class UpdateDeliveryStepOrderHandler(
     IReadRepository<Delivery> deliveryRepository,
     IValidator<UpdateDeliveryStepOrderCommand> validator,
     IApplicationTransaction transaction,
     IItineraryService itineraryService
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

          await delivery.ReorderStepsAsync(command.StepId, command.Order, itineraryService);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
