using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepCourierCommand(
     Guid DeliveryId,
     Guid StepId,
     Guid CourierId
) : ICommand<Result>;

public class AddDeliveryStepCourierHandler(
     IReadRepository<Delivery> deliveryRepository,
     IReadRepository<Courier> courierRepository,
     IApplicationTransaction transaction
) : ICommandHandler<AddDeliveryStepCourierCommand, Result>
{
     public async ValueTask<Result> Handle(AddDeliveryStepCourierCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          var courier = await courierRepository.FirstOrDefaultAsync(new CourierByIdSpecification(command.CourierId), cancellationToken);
          
          if (delivery is null || courier is null)
               return Result.NotFound();
          
          delivery.UpdateStepCourier(command.StepId, command.CourierId);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
