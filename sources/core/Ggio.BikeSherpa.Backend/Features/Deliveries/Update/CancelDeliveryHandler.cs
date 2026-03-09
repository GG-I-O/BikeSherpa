using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record CancelDeliveryCommand(
     Guid DeliveryId
) : ICommand<Result>;

[UsedImplicitly]
public class CancelDeliveryCommandValidator : AbstractValidator<CancelDeliveryCommand>
{
     public CancelDeliveryCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
     }
}

public class CancelDeliveryHandler(
     IReadRepository<Delivery> repository,
     IValidator<CancelDeliveryCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<CancelDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(CancelDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          if (delivery is null)
          {
               return Result.NotFound();
          }

          delivery.Cancel();

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
