using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public class DeliveryChangeOrderService(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction,
     IItinerarySpi itineraryService
) : IDeliveryChangeOrderService
{
     public async Task ChangeOrder(Delivery delivery, DeliveryStep step, int increment, CancellationToken cancellationToken)
     {
          if (step.CourierId is null)
               return;

          var courierId = (Guid)step.CourierId;

          var date = step.EstimatedDeliveryDate;

          var deliveries = await deliveryRepository.ListAsync(new DeliveryStepByCourierAndDate(courierId, date), cancellationToken);

          var steps = deliveries
               .SelectMany(x => x.Steps)
               .Where(s => s.CourierId == step.CourierId && s.EstimatedDeliveryDate.Date == step.EstimatedDeliveryDate.Date)
               .OrderBy(s => s.EstimatedDeliveryDate)
               .ToList();

          var stepIndex = steps.FindIndex(s => s.Id == step.Id);
          if (stepIndex + increment < 0 || stepIndex + increment >= steps.Count)
               return;

          var stepToMove = steps[stepIndex + increment];

          if (delivery.Steps.Any(s => s.Id == stepToMove.Id))
          {
               await delivery.ReorderStepsAsync(step.Id, step.Order + increment, itineraryService);
          }
          else
          {
               var oldDate = step.EstimatedDeliveryDate;
               delivery.UpdateStepDeliveryTime(step.Id, stepToMove.EstimatedDeliveryDate, false, true);
               deliveries.Find(d => d.Steps.Any(s => s.Id == stepToMove.Id))!
                    .UpdateStepDeliveryTime(stepToMove.Id, oldDate, false, true);
          }

          await transaction.CommitAsync(cancellationToken);
     }
}
