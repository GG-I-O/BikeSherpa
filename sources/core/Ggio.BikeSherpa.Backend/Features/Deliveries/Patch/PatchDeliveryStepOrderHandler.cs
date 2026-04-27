using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepOrderCommand(PatchDeliveryRequest Request) : ICommand<Result>;

public class PatchDeliveryStepOrderHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction,
     IItinerarySpi itineraryService
): ICommandHandler<PatchDeliveryStepOrderCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepOrderCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Request.DeliveryId), cancellationToken);
          if (delivery is null)
               return Result.NotFound();

          var rawValue = command.Request.Patches.Operations[0].value?.ToString();
          if (!int.TryParse(
                   rawValue,
                   out var newOrder))
          {
               return Result.Error("Invalid step order value provided.");
          }
          
          await delivery.ReorderStepsAsync(
               command.Request.StepId,
               newOrder,
               itineraryService
          );

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
