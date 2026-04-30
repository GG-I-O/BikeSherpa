using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Step;
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
     public async Task ChangeTime(DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken)
     {
          var steps = await GetDeliveriesAndSteps(step, date, cancellationToken);
          if (steps.Count == 0) return;
          
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
                    steps[stepIndex].UpdateEstimatedDeliveryDate(steps[stepIndex].EstimatedDeliveryDate + timeOffset);
               }
          }

          step.UpdateEstimatedDeliveryDate(date);
          await transaction.CommitAsync(cancellationToken);
     }
     
     public async Task ChangeOrder(DeliveryStep step, MoveDirection moveDirection, CancellationToken cancellationToken)
     {
          var steps = await GetDeliveriesAndSteps(step, step.EstimatedDeliveryDate, cancellationToken);
          if (steps.Count == 0) return;
          
          var increment = (int)moveDirection;
          
          var stepIndex = steps.FindIndex(s => s.Id == step.Id);
          if (stepIndex < 0 || stepIndex + increment < 0 || stepIndex + increment >= steps.Count)
               return;

          var stepToMove = steps[stepIndex + increment];

          if (step.ParentDelivery.Id == stepToMove.ParentDelivery.Id)
          {
               await step.ParentDelivery.ReorderStepsAsync(step.Id, step.Order + increment, itineraryService);
          }
          else
          {
               var oldDate = step.EstimatedDeliveryDate;
               step.ParentDelivery.UpdateStepDeliveryTime(step.Id, stepToMove.EstimatedDeliveryDate);
               stepToMove.ParentDelivery.UpdateStepDeliveryTime(stepToMove.Id, oldDate);
          }

          await transaction.CommitAsync(cancellationToken);
     }

     private async Task<List<DeliveryStep>> GetDeliveriesAndSteps(DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken)
     {
          if (step.CourierId is null)
               return [];

          var courierId = step.CourierId.Value;

          var deliveries = await deliveryRepository.ListAsync(new DeliveryStepByCourierAndDate(courierId, date), cancellationToken);

          var steps = deliveries
               .SelectMany(x => x.Steps)
               .Where(s => s.CourierId == step.CourierId && s.EstimatedDeliveryDate.Date == step.EstimatedDeliveryDate.Date)
               .OrderBy(s => s.EstimatedDeliveryDate)
               .ToList();

          return steps;
     }
}
