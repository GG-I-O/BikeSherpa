namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public enum DeliveryStatus
{
     New,
     Pending,
     Started,
     Completed,
     Cancelled
}

public enum DeliveryStatusTrigger
{
     Validate,
     Start,
     Complete,
     Cancel
}
