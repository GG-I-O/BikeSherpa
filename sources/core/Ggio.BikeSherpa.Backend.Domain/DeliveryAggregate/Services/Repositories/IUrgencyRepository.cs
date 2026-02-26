namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public interface IUrgencyRepository
{
     IReadOnlyList<Urgency> GetAll();
     public Urgency GetByName(string name);
}
