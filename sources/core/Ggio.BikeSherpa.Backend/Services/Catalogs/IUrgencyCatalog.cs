using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public interface IUrgencyCatalog
{
     IReadOnlyList<Urgency> Urgencies { get; }
     public Urgency GetUrgency(string name);
}
