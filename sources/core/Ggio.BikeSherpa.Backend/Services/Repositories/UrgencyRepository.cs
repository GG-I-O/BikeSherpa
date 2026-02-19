using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public class UrgencyRepository : IUrgencyRepository
{
     public IReadOnlyList<Urgency> Urgencies { get; }

     public UrgencyRepository(IEnumerable<Urgency> entities)
     {
          Urgencies = entities.Select(e => new Urgency(e.Id, e.Name, e.PriceCoefficient)).ToList();
     }

     public Urgency GetUrgency(string name)
     {
          return Urgencies.SingleOrDefault(u => u.Name == name)!;
     }
}
