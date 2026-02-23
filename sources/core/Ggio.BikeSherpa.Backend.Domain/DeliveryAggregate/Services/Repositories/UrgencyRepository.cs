namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public class UrgencyRepository : IUrgencyRepository
{
     public IReadOnlyList<Urgency> Urgencies { get; }

     public UrgencyRepository(IEnumerable<Urgency> entities)
     {
          Urgencies = entities.Select(e => new Urgency(e.Id, e.Name, e.PriceCoefficient)).ToList();
     }

     public Urgency GetUrgency(string name)
     {
          if (name == "")
          {
               throw new ArgumentException("Veuillez indiquer une urgence.");
          }
          else
          {
               return Urgencies.SingleOrDefault(u => string.Equals(u.Name, name, StringComparison.CurrentCultureIgnoreCase)) ?? throw new ArgumentException("Urgence inconnue.");
          }
     }
}
