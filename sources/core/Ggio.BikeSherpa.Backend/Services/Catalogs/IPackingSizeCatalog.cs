using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public interface IPackingSizeCatalog
{
     IReadOnlyList<PackingSize> PackingSizes { get; }
     PackingSize FromMeasurements(double weight, int length);
}

