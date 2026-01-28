using Facet;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Model;

[Facet(typeof(Courier), exclude:nameof(Courier.DomainEvents), NestedFacets = [typeof(AddressCrud)])]
public partial record CourierCrud;