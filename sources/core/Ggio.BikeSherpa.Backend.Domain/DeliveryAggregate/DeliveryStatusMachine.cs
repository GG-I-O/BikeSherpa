using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Stateless;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStatusMachine
{
     private readonly StateMachine<DeliveryStatusEnum, DeliveryStatusTrigger> _statusMachine;

     public DeliveryStatusMachine(Delivery delivery)
     {
          _statusMachine = new StateMachine<DeliveryStatusEnum, DeliveryStatusTrigger>(() => delivery.Status, status => delivery.Status = status);

          _statusMachine.Configure(DeliveryStatusEnum.Pending)
               .PermitIf(DeliveryStatusTrigger.Start, DeliveryStatusEnum.Started, () => delivery.Steps.Any(s => s is { StepType: StepTypeEnum.Pickup, Completed: true }))
               .Permit(DeliveryStatusTrigger.Cancel, DeliveryStatusEnum.Cancelled);

          _statusMachine.Configure(DeliveryStatusEnum.Started)
               .PermitIf(DeliveryStatusTrigger.Complete, DeliveryStatusEnum.Completed, () => delivery.Steps.All(s => s.Completed))
               .Permit(DeliveryStatusTrigger.Cancel, DeliveryStatusEnum.Cancelled);

          _statusMachine.Configure(DeliveryStatusEnum.Completed)
               .Ignore(DeliveryStatusTrigger.Complete)
               .Ignore(DeliveryStatusTrigger.Start);

          _statusMachine.Configure(DeliveryStatusEnum.Cancelled)
               .Ignore(DeliveryStatusTrigger.Complete)
               .Ignore(DeliveryStatusTrigger.Cancel);
     }

     public void Fire(DeliveryStatusTrigger trigger)
     {
          _statusMachine.Fire(trigger);
     }
}
