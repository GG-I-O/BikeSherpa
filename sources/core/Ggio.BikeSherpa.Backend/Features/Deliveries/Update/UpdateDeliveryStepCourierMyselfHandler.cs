using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using JetBrains.Annotations;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryStepCourierMyselfCommand(
     Guid DeliveryId,
     Guid StepId,
     string UserEmail
) : ICommand<Result>;

[UsedImplicitly]
public class UpdateDeliveryStepCourierMyselfCommandValidator : AbstractValidator<UpdateDeliveryStepCourierMyselfCommand>
{
     public UpdateDeliveryStepCourierMyselfCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepId).NotEmpty();
          RuleFor(x => x.UserEmail).NotEmpty().EmailAddress();
     }
}

public class UpdateDeliveryStepCourierMyselfHandler(
          IReadRepository<Delivery> deliveryRepository,
          IReadRepository<Courier> courierRepository,
          IValidator<UpdateDeliveryStepCourierMyselfCommand> validator,
          IApplicationTransaction transaction
     ) : ICommandHandler<UpdateDeliveryStepCourierMyselfCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryStepCourierMyselfCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          var courier = await courierRepository.FirstOrDefaultAsync(new CourierByEmailSpecification(command.UserEmail), cancellationToken);
          
          if (delivery is null || courier is null)
          {
               return Result.NotFound();
          }

          delivery.UpdateStepCourier(command.StepId, courier.Id);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
