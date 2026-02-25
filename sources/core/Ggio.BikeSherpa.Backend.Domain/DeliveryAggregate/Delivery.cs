using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.DddCore;

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

     public Delivery()
     {
          _statusMachine = new DeliveryStatusMachine(this);
     }

     public void GenerateReportId(Customer customer)
     {
          ReportId = $"{customer.Code}-{DateTime.UtcNow:yyyyMMddHHmmss}";
     }

     // Methods allowing to change the delivery status
     private void UpdateStatus()
     {
          switch (Status)
          {
               case DeliveryStatus.Pending:
                    if (Steps.Any(s => s is { StepType: StepType.Pickup, Completed: true }))
                    {
                         Start();
                    }
                    break;
               case DeliveryStatus.Started:
                    if (Steps.All(s => s.Completed))
                    {
                         Complete();
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

     private void Start()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Start);
          RegisterDomainEvent(new DeliveryStartedEvent(Id));
     }

     private void Complete()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Complete);
          RegisterDomainEvent(new DeliveryCompletedEvent(Id));
     }

     public void Cancel()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Cancel);
          RegisterDomainEvent(new DeliveryCancelledEvent(Id));
     }

     public void UpdateDeliveryStartDateTime(DateTimeOffset deliveryDateTime, IPricingStrategyService pricingStrategyService)
     {
          StartDate = deliveryDateTime;
          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);
     }

     public void ReorderSteps(Guid movedStepId, int newOrder)
     {
          var movedStep = Steps.Single(s => s.Id == movedStepId);
          Steps.Remove(movedStep);
          Steps.Insert(newOrder - 1, movedStep);
          var order = 1;
          foreach (var step in Steps)
          {
               step.Order = order++;
          }
     }

     private void ReorderStepsOnDeletion()
     {
          var order = 1;
          foreach (var step in Steps.OrderBy(s => s.Order))
          {
               step.Order = order++;
          }
     }

     public void UpdateStepCourier(Guid stepId, Guid courierId)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.CourierId = courierId;
     }

     public void UpdateStepDeliveryTime(Guid stepId, DateTimeOffset updatedDeliveryDate)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          var timeOffset = updatedDeliveryDate - existingStep.EstimatedDeliveryDate;
          existingStep.EstimatedDeliveryDate = updatedDeliveryDate;
          foreach (var step in Steps.Where(s => s.Order > existingStep.Order))
          {
               step.EstimatedDeliveryDate += timeOffset;
          }
     }

     public async Task UpdateStepCompletion(Guid stepId, bool completed)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.Completed = completed;
          if (completed)
          {
               existingStep.RealDeliveryDate = DateTimeOffset.UtcNow;
          }
          UpdateStatus();
     }

     public DeliveryStep AddStep(
          StepType stepType,
          Address stepAddress,
          double distance,
          IDeliveryZoneRepository deliveryZones,
          IPricingStrategyService pricingStrategyService)
     {
          var newStep = new DeliveryStep(
               stepType,
               Steps.Count + 1,
               stepAddress,
               distance)
          {
               Id = Guid.NewGuid(),
               StepAddress = stepAddress,
               StepZone = deliveryZones.GetByAddress(stepAddress.City)
          };

          if (Steps.Count >= 1)
          {
               var previousStep = Steps.Where(s => s.Order == newStep.Order - 1);
               newStep.EstimatedDeliveryDate = previousStep.Single().EstimatedDeliveryDate + TimeSpan.FromMinutes(15);
          }
          else
          {
               newStep.EstimatedDeliveryDate = StartDate;
          }

          Steps.Add(newStep);
          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);

          return newStep;
     }

     public void DeleteStep(
          DeliveryStep removedStep,
          IPricingStrategyService pricingStrategyService)
     {
          var nextStep = Steps.FirstOrDefault((s => s.Order == removedStep.Order + 1));
          var timeOffset = CalculateDeliveryTimeOffset(removedStep, nextStep);
          Steps.Remove(removedStep);
          ReorderStepsOnDeletion();
          UpdateStepDeliveryTimeOnDelete(removedStep.Order, timeOffset);
          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);
     }

     private TimeSpan CalculateDeliveryTimeOffset(DeliveryStep removedStep, DeliveryStep? nextStep)
     {
          return nextStep is null ? TimeSpan.Zero : nextStep.EstimatedDeliveryDate - removedStep.EstimatedDeliveryDate;
     }

     private void UpdateStepDeliveryTimeOnDelete(int stepOrder, TimeSpan timeOffset)
     {
          foreach (var step in Steps.Where(s => s.Order >= stepOrder))
          {
               step.EstimatedDeliveryDate += timeOffset;
          }
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

          // Update delivery price
          TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(this);
     }
}