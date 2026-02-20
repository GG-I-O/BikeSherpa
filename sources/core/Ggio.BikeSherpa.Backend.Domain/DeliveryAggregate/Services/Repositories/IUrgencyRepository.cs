namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public interface IUrgencyRepository
{
     IReadOnlyList<Urgency> Urgencies { get; }
     public Urgency GetUrgency(string name);
}
