using Ggio.BikeSherpa.Backend.Model;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Model;

public record CustomerDto: IHateoasLinks
{
     [UsedImplicitly]
     public required CustomerCrud Data { get; init; }
     public List<Link>? Links { get; set; }
}
