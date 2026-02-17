using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

[Facet(typeof(City))]
public partial class CityEntity
{
     public int Id { get; set; }
}
