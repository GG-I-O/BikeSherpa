using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record WaitingDeliveryCommand(Guid DeliveryId) : ICommand<Result>;

public class WaitingDeliveryCommandValidator : AbstractValidator<WaitingDeliveryCommand>
{
     public WaitingDeliveryCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
     }
}

public class WaitingDeliveryHandler(IReadRepository<Delivery> readRepository, IValidator<WaitingDeliveryCommand> validator, IApplicationTransaction applicationTransaction) : ICommandHandler<WaitingDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(WaitingDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var delivery = await readRepository.SingleOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
          {
               return Result.NotFound();
          }

          delivery.Waiting();
          await applicationTransaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
