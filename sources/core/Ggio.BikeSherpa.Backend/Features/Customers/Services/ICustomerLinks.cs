using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Services;

public interface ICustomerLinks
{
     List<Link> GenerateLinks(Guid id);
}
