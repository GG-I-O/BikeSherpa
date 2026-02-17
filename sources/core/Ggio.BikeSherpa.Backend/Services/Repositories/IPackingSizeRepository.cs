using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public interface IPackingSizeRepository
{
     IReadOnlyList<PackingSize> PackingSizes { get; }
     PackingSize FromName(string name);
}

