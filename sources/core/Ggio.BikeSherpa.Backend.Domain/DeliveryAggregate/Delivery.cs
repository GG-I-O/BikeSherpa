using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required PricingStrategy PricingStrategy { get; set; }
     public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public required string Urgency { get; set; }
     public double? TotalPrice { get; set; }
     public double? Discount { get; set; }
     public string? ReportId { get; set; }
     public required List<DeliveryStep> Steps { get; set; } = [];
     public string[] Details { get; set; } = [];
     public required string PackingSize { get; set; }
     public required bool InsulatedBox { get; set; }
     public required DateTimeOffset ContractDate { get; set; }
     public required DateTimeOffset StartDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     private readonly DeliveryStatusMachine _statusMachine;
     private readonly IMediator _mediator;

     public Delivery(IMediator mediator)
     {
          _statusMachine = new DeliveryStatusMachine(this);
          _mediator = mediator;
     }

     public string GenerateReportId(Customer customer)
     {
          var reportId = $"{customer.Code}-{DateTime.UtcNow:yyyyMMddHHmmss}";
          return reportId;
     }

     // Methods allowing to change the delivery status
     private async Task UpdateStatus()
     {
          switch (Status)
          {
               case DeliveryStatus.Pending:
                    if (Steps.Any(s => s is { StepType: StepType.Pickup, Completed: true }))
                    {
                         await Start();
                    }
                    break;
               case DeliveryStatus.Started:
                    if (Steps.All(s => s.Completed))
                    {
                         await Complete();
                    }
                    break;
               case DeliveryStatus.Completed:
                    throw new InvalidOperationException("Course déjà terminée.");
               case DeliveryStatus.Cancelled:
                    throw new InvalidOperationException("Course annulée.");
               default:
                    throw new ArgumentOutOfRangeException();
          }
     }

     private async Task Start()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Start);
          await _mediator.Publish(new DeliveryStartedEvent(Id));
     }

     private async Task Complete()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Complete);
          await _mediator.Publish(new DeliveryCompletedEvent(Id));
     }

     public async Task Cancel()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Cancel);
          await _mediator.Publish(new DeliveryCancelledEvent(Id));
     }

     // Methods allowing to change delivery properties
     public void UpdateDeliveryStartDateTime(DateTimeOffset deliveryDateTime, IPricingStrategyService pricingStrategyService)
     {
          StartDate = deliveryDateTime;
          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);
     }

     // Methods allowing to update delivery steps
     public void UpdateStepOrder(Guid stepId, int order)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.Order = order;
     }

     public void UpdateStepCourier(Guid stepId, Guid courierId)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.CourierId = courierId;
     }

     private void UpdateStepDeliveryTime(Guid stepId, DateTimeOffset estimatedDeliveryDate)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.EstimatedDeliveryDate = estimatedDeliveryDate;
     }

     public async Task UpdateStepCompletion(Guid stepId, bool completed)
     {
          try
          {
               var existingStep = Steps.Single(s => s.Id == stepId);
               existingStep.Completed = completed;
               await UpdateStatus();
          }
          catch (Exception e)
          {
               Console.WriteLine(e);
               throw;
          }
     }

     private bool StepCanFollow(StepType previousStep, StepType currentStep)
     {
          return previousStep == StepType.Pickup && currentStep == StepType.Dropoff || previousStep == StepType.Dropoff && currentStep == StepType.Dropoff || previousStep == StepType.Dropoff && currentStep == StepType.Pickup;
     }

     public DeliveryStep AddStep(
          StepType stepType,
          int order,
          Address stepAddress,
          double distance,
          DateTimeOffset estimatedDeliveryDate,
          IDeliveryZoneRepository deliveryZones,
          IPricingStrategyService pricingStrategyService)
     {
          var newStep = new DeliveryStep(
               stepType,
               order,
               stepAddress,
               deliveryZones.GetByAddress(stepAddress.City),
               distance,
               estimatedDeliveryDate)
          {
               Id = Guid.NewGuid()
          };

          Steps.Add(newStep);

          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);

          return newStep;
     }

     public void DeleteStep(
          DeliveryStep step,
          IPricingStrategyService pricingStrategyService)
     {
          Steps.Remove(step);
          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);
     }

     public void UpdateSteps(List<DeliveryStep> steps, IDeliveryZoneRepository deliveryZones, IPricingStrategyService pricingStrategyService)
     {
          foreach (var step in steps)
          {
               var existing = Steps.FirstOrDefault(s => s.Id == step.Id);

               if (existing is null)
               {
                    step.StepZone = deliveryZones.GetByAddress(step.StepAddress.City);
                    Steps.Add(step);
               }

               else
               {
                    existing.Update(
                         step.StepType,
                         step.Order,
                         step.Completed,
                         step.StepAddress,
                         deliveryZones.GetByAddress(step.StepAddress.City),
                         step.Distance,
                         step.EstimatedDeliveryDate);
               }
          }

          DeleteOldSteps(steps);

          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);
     }

     private void DeleteOldSteps(List<DeliveryStep> steps)
     {
          var incomingIds = steps.Select(s => s.Id).ToHashSet();
          Steps.RemoveAll(s => !incomingIds.Contains(s.Id));
     }
}