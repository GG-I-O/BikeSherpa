using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public class UrgencyRepository : IUrgencyRepository
{
     public IReadOnlyList<Urgency> Urgencies { get; }

     public UrgencyRepository(IEnumerable<UrgencyEntity> entities)
     {
          Urgencies = entities.Select(e => new Urgency(e.Name, e.PriceCoefficient)).ToList();
     }

     public Urgency GetUrgency(string name)
     {
          return Urgencies.SingleOrDefault(u => u.Name == name)!;
     }
}
