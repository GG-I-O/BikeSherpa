namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class DeliveryStatus : Enumeration
{
     private readonly static DeliveryStatus Pending = new(1, "En attente");
     private readonly static DeliveryStatus Started = new(2, "Commencée");
     private readonly static DeliveryStatus Completed = new(3, "Terminée");
     private readonly static DeliveryStatus Canceled = new(4, "Annulée");

     private DeliveryStatus(int id, string name) : base(id, name) { }

     public bool CanTransitionTo(DeliveryStatus next)
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
