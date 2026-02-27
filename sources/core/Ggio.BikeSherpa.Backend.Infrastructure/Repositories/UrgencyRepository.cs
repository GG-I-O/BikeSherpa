using Ardalis.GuardClauses;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class UrgencyRepository(BackendDbContext context) : IUrgencyRepository
{
     public IReadOnlyList<Urgency> GetAll()
     {
          return context.Urgencies.ToList();
     }

     public Urgency GetByName(string name)
     {
          Guard.Against.NullOrEmpty(name);

          return context.Urgencies.SingleOrDefault(u => string.Equals(u.Name, name, StringComparison.CurrentCultureIgnoreCase)) ?? throw new ArgumentException("Urgence inconnue.", nameof(name));
     }
}
