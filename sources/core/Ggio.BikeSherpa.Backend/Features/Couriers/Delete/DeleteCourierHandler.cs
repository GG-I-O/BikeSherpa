using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Delete;

public record DeleteCourierCommand(
     Guid Id
) : ICommand<Result>;

public class DeleteCourierHandler(
     ICourierDeleteEventHandler courierDeleteEventHandler,
     IReadRepository<Courier> repository,
     IApplicationTransaction transaction
) : ICommandHandler<DeleteCourierCommand, Result>
{
     public async ValueTask<Result> Handle(DeleteCourierCommand command, CancellationToken cancellationToken)
     {
          var entity = await repository.FirstOrDefaultAsync(new CourierByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          await courierDeleteEventHandler.DeleteCourierAsync(entity, cancellationToken);
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
