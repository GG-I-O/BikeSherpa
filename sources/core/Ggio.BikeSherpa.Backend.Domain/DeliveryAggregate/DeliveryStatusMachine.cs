using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Stateless;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStatusMachine
{
     private readonly StateMachine<DeliveryStatusEnum, DeliveryStatusTrigger> _statusMachine;

     public DeliveryStatusMachine(Delivery delivery)
     {
          _statusMachine = new StateMachine<DeliveryStatusEnum, DeliveryStatusTrigger>(() => delivery.Status!, status => delivery.Status = status);

          _statusMachine.Configure(DeliveryStatusEnum.Pending)
               .Permit(DeliveryStatusTrigger.Start, DeliveryStatusEnum.Started)
               .Permit(DeliveryStatusTrigger.Cancel, DeliveryStatusEnum.Cancelled);

          _statusMachine.Configure(DeliveryStatusEnum.Started)
               .SubstateOf(DeliveryStatusEnum.Pending)
               .Permit(DeliveryStatusTrigger.Complete, DeliveryStatusEnum.Completed)
               .Permit(DeliveryStatusTrigger.Cancel, DeliveryStatusEnum.Cancelled);

          _statusMachine.Configure(DeliveryStatusEnum.Completed)
               .SubstateOf(DeliveryStatusEnum.Started);
          
          _statusMachine.Configure(DeliveryStatusEnum.Pending)
               .PermitIf(DeliveryStatusTrigger.Start, DeliveryStatusEnum.Started, () => delivery.Steps.Any(s => s.Completed == true));
          
          _statusMachine.Configure(DeliveryStatusEnum.Started)
               .PermitIf(DeliveryStatusTrigger.Complete, DeliveryStatusEnum.Completed, () => delivery.Steps.All(s => s.Completed == true));
     }

     public void Fire(DeliveryStatusTrigger trigger)
     {
          _statusMachine.Fire(trigger);
     }
}
