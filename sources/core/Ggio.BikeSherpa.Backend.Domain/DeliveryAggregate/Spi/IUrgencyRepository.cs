namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

public interface IUrgencyRepository
{
     IReadOnlyList<Urgency> GetAll();
     public Urgency? GetByName(string name);
}
