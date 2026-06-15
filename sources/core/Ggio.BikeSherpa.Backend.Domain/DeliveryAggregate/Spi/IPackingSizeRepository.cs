namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

public interface IPackingSizeRepository
{
     IReadOnlyList<PackingSize> GetAll();
     PackingSize? GetByName(string name);
}
