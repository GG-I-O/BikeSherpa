using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public interface IDeliveryEvent : IDomainEvent
{
     public Guid DeliveryId { get; }
}
