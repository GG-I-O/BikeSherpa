using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepCommentCommand(PatchDeliveryRequest Request) : ICommand<Result>;

public class PatchDeliveryStepCommentHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction
     ) : ICommandHandler<PatchDeliveryStepCommentCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepCommentCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Request.DeliveryId), cancellationToken);
          if (delivery is null)
               return Result.NotFound();
          
          var step = delivery.Steps.SingleOrDefault(s => s.Id == command.Request.StepId);
          if (step is null)
               return Result.NotFound();
          
          var comment = command.Request.Patches.Operations[0].value?.ToString();
          step.Comment = comment;

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
