using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Stateless;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStatusMachine
{
     private readonly StateMachine<DeliveryStatus, DeliveryStatusTrigger> _statusMachine;

     public DeliveryStatusMachine(Delivery delivery)
     {
          _statusMachine = new StateMachine<DeliveryStatus, DeliveryStatusTrigger>(() => delivery.Status, status => delivery.Status = status);

          _statusMachine.Configure(DeliveryStatus.Pending)
               .PermitIf(DeliveryStatusTrigger.Start, DeliveryStatus.Started, () => delivery.Steps.Any(s => s is { StepType: StepType.Pickup, Completed: true }))
               .Permit(DeliveryStatusTrigger.Cancel, DeliveryStatus.Cancelled);

          _statusMachine.Configure(DeliveryStatus.Started)
               .PermitIf(DeliveryStatusTrigger.Complete, DeliveryStatus.Completed, () => delivery.Steps.All(s => s.Completed))
               .Permit(DeliveryStatusTrigger.Cancel, DeliveryStatus.Cancelled);

          _statusMachine.Configure(DeliveryStatus.Completed)
               .Ignore(DeliveryStatusTrigger.Complete)
               .Ignore(DeliveryStatusTrigger.Start);

          _statusMachine.Configure(DeliveryStatus.Cancelled)
               .Ignore(DeliveryStatusTrigger.Complete)
               .Ignore(DeliveryStatusTrigger.Cancel);
     }

     public void Fire(DeliveryStatusTrigger trigger)
     {
          _statusMachine.Fire(trigger);
     }
}
