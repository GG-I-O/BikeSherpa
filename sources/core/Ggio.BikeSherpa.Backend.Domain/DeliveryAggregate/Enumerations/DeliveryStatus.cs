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
     Waiting,
     Validate,
     Start,
     Complete,
     Cancel
}
