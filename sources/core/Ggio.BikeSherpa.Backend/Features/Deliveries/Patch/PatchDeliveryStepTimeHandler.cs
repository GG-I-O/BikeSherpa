using System.Globalization;
using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepTimeCommand(PatchDeliveryRequest Request) : ICommand<Result>;

public class PatchDeliveryStepTimeHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction
): ICommandHandler<PatchDeliveryStepTimeCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepTimeCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Request.DeliveryId), cancellationToken);
          if (delivery is null)
               return Result.NotFound();

          var rawValue = command.Request.Patches.Operations[0].value?.ToString();
          if (!DateTimeOffset.TryParse(
                   rawValue,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.RoundtripKind,
                   out var estimatedDeliveryDate))
          {
               return Result.Error("estimatedDeliveryDate must be a valid ISO-8601 datetime.");
          }

          delivery.UpdateStepDeliveryTime(
               command.Request.StepId,
               estimatedDeliveryDate
          );

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
