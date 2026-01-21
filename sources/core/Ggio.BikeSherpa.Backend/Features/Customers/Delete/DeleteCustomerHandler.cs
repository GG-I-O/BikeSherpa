using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Delete;

public record DeleteCustomerCommand(
     Guid Id
     ) : ICommand<Result>;

public class DeleteCustomerHandler(
     ICustomerDeleteEventHandler customerDeleteEventHandler,
     IReadRepository<Customer> repository,
     IApplicationTransaction transaction
     ) : ICommandHandler<DeleteCustomerCommand, Result>
{
     public async ValueTask<Result> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
     {
          var entity = await repository.FirstOrDefaultAsync(new CustomerByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          await customerDeleteEventHandler.DeleteCustomerAsync(entity, cancellationToken);
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
