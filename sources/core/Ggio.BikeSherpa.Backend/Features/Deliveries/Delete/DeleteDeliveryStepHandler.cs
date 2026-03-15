using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;

public record DeleteDeliveryStepCommand(
     Guid DeliveryId,
     Guid StepId
) : ICommand<Result>;

public class DeleteDeliveryStepHandler(
     IReadRepository<Delivery> repository,
     IApplicationTransaction transaction,
     IPricingStrategyService pricingStrategyService,
     IItineraryService itineraryService
) : ICommandHandler<DeleteDeliveryStepCommand, Result>
{
     public async ValueTask<Result> Handle(DeleteDeliveryStepCommand command, CancellationToken cancellationToken)
     {
          var entity = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          var step = entity?.Steps.SingleOrDefault(s => s.Id == command.StepId);
          if (entity is null || step is null)
          {
               return Result.NotFound();
          }

          await entity.DeleteStepAsync(step, pricingStrategyService, itineraryService);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
