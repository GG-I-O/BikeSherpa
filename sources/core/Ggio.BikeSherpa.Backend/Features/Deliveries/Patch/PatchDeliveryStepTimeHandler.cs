using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepTimeCommand(
     Guid DeliveryId,
     Guid StepId,
     DateTimeOffset EstimatedDeliveryDate
     ) : ICommand<Result>;

public class PatchDeliveryStepTimeHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction
): ICommandHandler<PatchDeliveryStepTimeCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepTimeCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
               return Result.NotFound();

          delivery.UpdateStepDeliveryTime(
               command.StepId,
               command.EstimatedDeliveryDate
          );

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
