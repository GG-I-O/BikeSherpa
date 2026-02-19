using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public interface IUrgencyRepository
{
     IReadOnlyList<Urgency> Urgencies { get; }
     public Urgency GetUrgency(string name);
}
