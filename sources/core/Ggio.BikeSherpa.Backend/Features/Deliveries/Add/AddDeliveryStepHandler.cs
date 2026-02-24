using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepCommand(
     Guid DeliveryId,
     StepTypeEnum StepType,
     int Order,
     Address StepAddress,
     double Distance,
     DateTimeOffset EstimatedDeliveryDate
     ) : ICommand<Result<Guid>>;

public class AddDeliveryStepCommandValidator : AbstractValidator<AddDeliveryStepCommand>
{
     public AddDeliveryStepCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
          RuleFor(x => x.StepType).NotEmpty();
          RuleFor(x => x.Order).NotEmpty();
          RuleFor(x => x.StepAddress).NotEmpty();
          RuleFor(x => x.Distance).NotEmpty();
          RuleFor(x => x.EstimatedDeliveryDate).NotEmpty();
     }
}

public class AddDeliveryStepHandler(
     IValidator<AddDeliveryStepCommand> validator,
     IApplicationTransaction transaction,
     IReadRepository<Delivery> deliveryRepository,
     IDeliveryZoneRepository deliveryZones
     ) : ICommandHandler<AddDeliveryStepCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryStepCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          if (delivery is null)
               return Result<Guid>.NotFound();

          var deliveryStep = new DeliveryStep(
               command.StepType,
               command.Order,
               command.StepAddress,
               deliveryZones.FromAddress(command.StepAddress.City),
               command.Distance,
               command.EstimatedDeliveryDate)
          {
               Id = Guid.NewGuid()
          };

          delivery.Steps.Add(deliveryStep);

          await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(deliveryStep.Id);
     }
}
