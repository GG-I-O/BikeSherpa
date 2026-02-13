using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public interface IPackingSizeCatalog
{
     IReadOnlyList<PackingSize> PackingSizes { get; }
     PackingSize FromMeasurements(PricingStrategyEnum pricingStrategy, double totalWeight, int highestPackageLength);
}

