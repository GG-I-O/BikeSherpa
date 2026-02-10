namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class StepType : Enumeration
{
     public readonly static StepType Pickup = new(1, "Collecte");
     public readonly static StepType Dropoff = new(2, "Dépôt");

     private StepType(int id, string name) : base(id, name) { }

     public bool CanFollow(StepType previous) =>
          previous == Pickup && this == Dropoff
          || previous == Dropoff && this == Dropoff;
}
