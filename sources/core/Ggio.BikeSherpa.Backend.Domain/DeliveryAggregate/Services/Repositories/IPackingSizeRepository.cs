namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public interface IPackingSizeRepository
{
     IReadOnlyList<PackingSize> GetAll();
     PackingSize? GetByName(string name);
}
