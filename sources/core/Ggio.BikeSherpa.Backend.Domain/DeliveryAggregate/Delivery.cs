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
          await _mediator.Publish(new DeliveryStartedEvent(this));
     }

     private void Complete()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Complete);
          await _mediator.Publish(new DeliveryCompletedEvent(this));
     }
     
     public async Task Cancel()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Cancel);
          await _mediator.Publish(new DeliveryCancelledEvent(this));
     }
     
     // Methods allowing to change delivery properties
     public void UpdateDeliveryStartDateTime(DateTimeOffset deliveryDateTime)
     {
          StartDate = deliveryDateTime;
     }
     
     public void SetDeliveryPrice(double deliveryPrice)
     {
          TotalPrice = deliveryPrice;
     }
     
     private void AddStep(StepTypeEnum stepTypeEnum, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var step = new DeliveryStep(stepTypeEnum, Steps.Count + 1, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
          Steps.Add(step);
     }
     
     // Methods allowing to update delivery steps
     private void UpdateStepOrder(Guid stepId, int order)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.CourierId = courierId;
     }
     
     private void UpdateStepCourier(Guid stepId, Guid courierId)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.AssignCourier(courierId);
     }
     
     private void UpdateStepDeliveryTime(Guid stepId, DateTimeOffset estimatedDeliveryDate)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.UpdateDeliveryTime(estimatedDeliveryDate);
     }

     private void UpdateStepCompletion(Guid stepId, bool completed)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.UpdateCompletion(completed);
     }
     
     private bool StepCanFollow(StepTypeEnum previousStep, StepTypeEnum currentStep) =>
          previousStep == StepTypeEnum.Pickup && currentStep == StepTypeEnum.Dropoff
          || previousStep == StepTypeEnum.Dropoff && currentStep == StepTypeEnum.Dropoff;

     private void DeleteStep(Guid stepId)
     {
          var incomingIds = steps.Select(s => s.Id).ToHashSet();
          Steps.RemoveAll(s => !incomingIds.Contains(s.Id));
     }
}