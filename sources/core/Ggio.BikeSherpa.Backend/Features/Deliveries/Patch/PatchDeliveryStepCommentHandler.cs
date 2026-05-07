using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepCommentCommand(
     Guid DeliveryId,
     Guid StepId,
     string? Comment
     ) : ICommand<Result>;

public class PatchDeliveryStepCommentHandler(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction
     ) : ICommandHandler<PatchDeliveryStepCommentCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepCommentCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
               return Result.NotFound();
          
          var step = delivery.Steps.SingleOrDefault(s => s.Id == command.StepId);
          if (step is null)
               return Result.NotFound();
          
          step.Comment = command.Comment;

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
