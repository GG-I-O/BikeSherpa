namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public interface IPackingSizeRepository
{
     IReadOnlyList<PackingSize> PackingSizes { get; }
     PackingSize FromName(string name);
}
