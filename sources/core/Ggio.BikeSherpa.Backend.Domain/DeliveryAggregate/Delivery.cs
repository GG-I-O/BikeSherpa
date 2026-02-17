using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required PricingStrategyEnum PricingStrategy { get; set; }
     public DeliveryStatusEnum Status { get; set; } = DeliveryStatusEnum.Pending;
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public required string Urgency { get; set; }
     public double TotalPrice { get; set; }
     public required Guid ReportId { get; set; }
     public required List<DeliveryStep> Steps { get; set; } = [];
     public string[]? Details { get; set; } = [];
     public required string PackingSize { get; set; }
     public required bool InsulatedBox { get; set; }
     public required bool ExactTime { get; set; }
     public required bool ReturnJourney { get; set; }
     public required DateTimeOffset ContractDate { get; set; }
     public required DateTimeOffset StartDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
     
     public void Start()
     {
          var statusMachine = new DeliveryStatusMachine(this);
          statusMachine.Fire(DeliveryStatusTrigger.Start);
     }

     public void Complete()
     {
          var statusMachine = new DeliveryStatusMachine(this);
          statusMachine.Fire(DeliveryStatusTrigger.Complete);
     }
     
     public void Cancel()
     {
          var statusMachine = new DeliveryStatusMachine(this);
          statusMachine.Fire(DeliveryStatusTrigger.Cancel);
     }
     
     private void AddStep(StepTypeEnum stepTypeEnum, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var step = new DeliveryStep(stepTypeEnum, Steps.Count + 1, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
          Steps.Add(step);
     }
     
     private void UpdateStepOrder(Guid stepId, int order)
     {
          var existingStep = Steps.Single(s => s.Id == stepId);
          existingStep.UpdateOrder(order);
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
          var step = Steps.Single(s => s.Id == stepId);
          Steps.Remove(step);
     }

     public void SetDeliveryPrice(double deliveryPrice)
     {
          TotalPrice = deliveryPrice;
     }
}

