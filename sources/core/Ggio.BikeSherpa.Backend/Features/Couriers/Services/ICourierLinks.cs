using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Services;

public interface ICourierLinks
{
     List<Link> GenerateLinks(Guid id);
}