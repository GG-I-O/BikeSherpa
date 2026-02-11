namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class StepTypeEnum : Enumeration
{
     public readonly static StepTypeEnum Pickup = new(1, "Collecte");
     public readonly static StepTypeEnum Dropoff = new(2, "Dépôt");

     private StepTypeEnum(int id, string name) : base(id, name) { }

     public bool CanFollow(StepTypeEnum previous) =>
          previous == Pickup && this == Dropoff
          || previous == Dropoff && this == Dropoff;
}
