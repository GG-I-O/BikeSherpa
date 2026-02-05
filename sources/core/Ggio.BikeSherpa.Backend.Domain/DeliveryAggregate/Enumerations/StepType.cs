namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class StepType : Enumeration
{
     private readonly static StepType Pickup = new(1, "Pickup");
     private readonly static StepType Dropoff = new(2, "Dropoff");

     private StepType(int id, string name) : base(id, name) { }

     public bool CanFollow(StepType previous) =>
          previous == Pickup && this == Dropoff
          || previous == Dropoff && this == Dropoff;
}
