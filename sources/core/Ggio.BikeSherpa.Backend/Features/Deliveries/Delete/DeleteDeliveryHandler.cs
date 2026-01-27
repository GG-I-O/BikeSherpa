using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;

public record DeleteDeliveryCommand(
     Guid Id
) : ICommand<Result>;

public class DeleteDeliveryHandler(
     IDeliveryDeleteEventHandler deliveryDeleteEventHandler,
     IReadRepository<Delivery> repository,
     IApplicationTransaction transaction
     ) : ICommandHandler<DeleteDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(DeleteDeliveryCommand command, CancellationToken cancellationToken)
     {
          var entity = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          await deliveryDeleteEventHandler.DeleteDeliveryAsync(entity, cancellationToken);
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}