using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record ValidateDeliveryCommand(Guid DeliveryId) : ICommand<Result>;

public class ValidateDeliveryCommandValidator : AbstractValidator<ValidateDeliveryCommand>
{
     public ValidateDeliveryCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
     }
}

public class ValidateDeliveryHandler( IReadRepository<Delivery> repository,
     IValidator<ValidateDeliveryCommand> validator,
     IApplicationTransaction transaction) : ICommandHandler<ValidateDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(ValidateDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          
          var delivery = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
          {
               return Result.NotFound();
          }
          
          delivery.Validate();
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
