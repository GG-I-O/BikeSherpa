using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public class DeliveryChangeTimeService(
     IReadRepository<Delivery> deliveryRepository,
     IApplicationTransaction transaction,
     IItinerarySpi itineraryService
) : IDeliveryChangeTimeService
{
     public async Task ChangeTime(Delivery delivery, DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken)
     {
          var (deliveries, steps) = await GetDeliveriesAndSteps(step, date, cancellationToken);
          
          var stepIndex = steps.FindIndex(s => s.Id == step.Id);
          if (stepIndex < 0 || stepIndex >= steps.Count)
               return;
          
          var timeOffset = date - step.EstimatedDeliveryDate;

          // If it does not disrupt the order, we change following step time
          // If not, we will only change the step asked and nothing else
          if (stepIndex - 1 <= 0 || !(steps[stepIndex - 1].EstimatedDeliveryDate > step.EstimatedDeliveryDate + timeOffset))
          {
               stepIndex++;
               for (; stepIndex < steps.Count; stepIndex++)
               {
                    var stepToMove = steps[stepIndex];
                    deliveries.Find(d => d.Steps.Any(s => s.Id == stepToMove.Id))!
                         .UpdateStepDeliveryTime(
                              stepToMove.Id,
                              stepToMove.EstimatedDeliveryDate + timeOffset,
                              false,
                              true
                         );
               }
          }

          delivery.UpdateStepDeliveryTime(step.Id, date, false, true);
          await transaction.CommitAsync(cancellationToken);
     }
     
     public async Task ChangeOrder(Delivery delivery, DeliveryStep step, int increment, CancellationToken cancellationToken)
     {
          var (deliveries, steps) = await GetDeliveriesAndSteps(step, step.EstimatedDeliveryDate, cancellationToken);
          
          var stepIndex = steps.FindIndex(s => s.Id == step.Id);
          if (stepIndex < 0 || stepIndex + increment < 0 || stepIndex + increment >= steps.Count)
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

     private async Task<(List<Delivery>, List<DeliveryStep>)> GetDeliveriesAndSteps(DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken)
     {
          if (step.CourierId is null)
               return ([], []);

          var courierId = (Guid)step.CourierId;

          var deliveries = await deliveryRepository.ListAsync(new DeliveryStepByCourierAndDate(courierId, date), cancellationToken);

          var steps = deliveries
               .SelectMany(x => x.Steps)
               .Where(s => s.CourierId == step.CourierId && s.EstimatedDeliveryDate.Date == step.EstimatedDeliveryDate.Date)
               .OrderBy(s => s.EstimatedDeliveryDate)
               .ToList();

          return (deliveries, steps);
     }
}
