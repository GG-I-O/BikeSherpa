using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record RenewDeliveryCommand(Guid DeliveryId) : ICommand<Result>;

public class RenewDeliveryCommandValidator : AbstractValidator<RenewDeliveryCommand>
{
     public RenewDeliveryCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
     }
}

public class RenewDeliveryHandler(IReadRepository<Delivery> readRepository, IValidator<RenewDeliveryCommand> validator, IApplicationTransaction applicationTransaction) : ICommandHandler<RenewDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(RenewDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var delivery = await readRepository.SingleOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
          {
               return Result.NotFound();
          }

          delivery.Renew();
          await applicationTransaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
