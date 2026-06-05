using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepOrderCommand(
     Guid DeliveryId,
     Guid StepId,
     int Order
) : ICommand<Result>;

public class PatchDeliveryStepOrderHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction,
     IItinerarySpi itineraryService
) : ICommandHandler<PatchDeliveryStepOrderCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepOrderCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
          {
               return Result.NotFound();
          }

          await delivery.ReorderStepsAsync(
               command.StepId,
               command.Order,
               itineraryService
          );

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
