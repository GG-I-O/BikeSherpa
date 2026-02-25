using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryStepCompletionRequest(bool Completed);

public record UpdateDeliveryStepCompletionCommand(
     Guid DeliveryId,
     Guid StepId,
     bool Completed
) : ICommand<Result>;

public class UpdateDeliveryStepCompletionCommandValidator : AbstractValidator<UpdateDeliveryStepCompletionCommand>
{
     public UpdateDeliveryStepCompletionCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
     }
}

public class UpdateDeliveryStepCompletionHandler(
     IReadRepository<Delivery> deliveryRepository,
     IValidator<UpdateDeliveryStepCompletionCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateDeliveryStepCompletionCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryStepCompletionCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          if (delivery is null)
          {
               return Result.NotFound();
          }

          delivery.UpdateStepCompletion(command.StepId, command.Completed);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
