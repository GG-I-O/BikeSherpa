namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class DeliveryStatusEnum : Enumeration
{
     private readonly static DeliveryStatusEnum Pending = new(1, "En attente");
     private readonly static DeliveryStatusEnum Started = new(2, "Commencée");
     private readonly static DeliveryStatusEnum Completed = new(3, "Terminée");
     private readonly static DeliveryStatusEnum Canceled = new(4, "Annulée");

     private DeliveryStatusEnum(int id, string name) : base(id, name) { }

     public bool CanTransitionTo(DeliveryStatusEnum next)
     {
          return (this, next) switch
          {
               var (currentStatus, nextStatus) when currentStatus == nextStatus => true,
               var (currentStatus, nextStatus) when currentStatus == Pending && nextStatus == Started => true,
               var (currentStatus, nextStatus) when currentStatus == Started && (nextStatus == Completed || nextStatus == Canceled) => true,
               _ => false
          };
     }
}
