using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public record PatchDeliveryStepCourierCommentCommand(
    Guid DeliveryId,
    Guid StepId,
    string? CourierComment
) : ICommand<Result>;

public class PatchDeliveryStepCourierCommentHandler(
    IReadRepository<Delivery> deliveryRepository,
    IApplicationTransaction transaction
    ) : ICommandHandler<PatchDeliveryStepCourierCommentCommand, Result>
{
     public async ValueTask<Result> Handle(PatchDeliveryStepCourierCommentCommand command, CancellationToken cancellationToken)
     {
         var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
         if (delivery is null)
             return Result.NotFound();
          
         var step = delivery.Steps.SingleOrDefault(s => s.Id == command.StepId);
         if (step is null)
             return Result.NotFound();
          
         step.CourierComment = command.CourierComment;

         await transaction.CommitAsync(cancellationToken);
         return Result.Success();
     }
}
