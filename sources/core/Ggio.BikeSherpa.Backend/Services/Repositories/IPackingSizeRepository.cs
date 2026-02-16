using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public interface IPackingSizeRepository
{
     IReadOnlyList<PackingSize> PackingSizes { get; }
     PackingSize FromMeasurements(PricingStrategyEnum pricingStrategy, double totalWeight, int highestPackageLength);
}

