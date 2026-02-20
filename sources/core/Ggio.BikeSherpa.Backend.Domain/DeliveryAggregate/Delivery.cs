using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required PricingStrategyEnum PricingStrategy { get; set; }
     public DeliveryStatusEnum Status { get; set; } = DeliveryStatusEnum.Pending;
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public required string Urgency { get; set; }
     public double? TotalPrice { get; set; }
     public double? Discount { get; set; }
     public required Guid ReportId { get; set; }
     public required List<DeliveryStep> Steps { get; set; } = [];
     public string[]? Details { get; set; } = [];
     public required string PackingSize { get; set; }
     public required bool InsulatedBox { get; set; }
     public required bool ExactTime { get; set; }
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
     
     // Methods allowing to change the delivery status
     public async Task Start()
     {
          _statusMachine.Fire(DeliveryStatusTrigger.Start);
          await _mediator.Publish(new DeliveryStartedEvent(this));
     }

     public async Task Complete()
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
}