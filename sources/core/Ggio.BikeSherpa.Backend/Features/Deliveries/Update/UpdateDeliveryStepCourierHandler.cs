using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryStepCourierRequest(Guid CourierId);

public record UpdateDeliveryStepCourierCommand(
     Guid DeliveryId,
     Guid StepId,
     Guid CourierId
) : ICommand<Result>;

public class UpdateDeliveryStepCourierCommandValidator : AbstractValidator<UpdateDeliveryStepCourierCommand>
{
     public UpdateDeliveryStepCourierCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
          RuleFor(x => x.CourierId).NotEmpty();
     }
}

public class UpdateDeliveryStepCourierHandler(
          IReadRepository<Delivery> deliveryRepository,
          IReadRepository<Courier> courierRepository,
          IValidator<UpdateDeliveryStepCourierCommand> validator,
          IApplicationTransaction transaction
     ) : ICommandHandler<UpdateDeliveryStepCourierCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryStepCourierCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          var courier = await courierRepository.FirstOrDefaultAsync(new CourierByIdSpecification(command.CourierId), cancellationToken);

          if (delivery is null || courier is null)
          {
               return Result.NotFound();
          }

          delivery.UpdateStepCourier(command.StepId, command.CourierId);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
