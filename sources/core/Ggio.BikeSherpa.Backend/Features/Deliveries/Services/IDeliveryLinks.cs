using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public interface IDeliveryLinks
{
     List<Link> GenerateLinks(Guid id);
}
