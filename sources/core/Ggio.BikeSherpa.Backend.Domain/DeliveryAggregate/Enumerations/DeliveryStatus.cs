namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public enum DeliveryStatus
{
     Pending,
     Started,
     Completed,
     Cancelled
}

public enum DeliveryStatusTrigger
{
     Start,
     Complete,
     Cancel
}
