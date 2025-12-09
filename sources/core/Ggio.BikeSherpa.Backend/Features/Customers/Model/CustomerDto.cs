using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Model;

public record CustomerDto: IHateoasLinks
{
     public required CustomerCrud Data;
     public List<Link>? Links { get; set; }
}
