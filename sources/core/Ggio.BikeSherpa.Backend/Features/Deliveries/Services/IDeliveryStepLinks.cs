using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public interface IDeliveryStepLinks
{
     List<Link> GenerateLinks(Guid deliveryId, Guid stepId);
}
