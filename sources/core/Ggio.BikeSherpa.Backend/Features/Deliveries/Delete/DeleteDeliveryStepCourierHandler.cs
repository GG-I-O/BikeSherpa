using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;

public record DeleteDeliveryStepCourierCommand(
     Guid DeliveryId,
     Guid StepId
) : ICommand<Result>;

public class DeleteDeliveryStepCourierHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction
) : ICommandHandler<DeleteDeliveryStepCourierCommand, Result>
{
     public async ValueTask<Result> Handle(DeleteDeliveryStepCourierCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          
          if (delivery is null)
               return Result.NotFound();
          
          delivery.UpdateStepCourier(command.StepId, null);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
