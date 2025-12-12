using Facet;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Customers;

[Facet(typeof(Customer), exclude:nameof(Customer.DomainEvents))]
public partial record CustomerCrud;