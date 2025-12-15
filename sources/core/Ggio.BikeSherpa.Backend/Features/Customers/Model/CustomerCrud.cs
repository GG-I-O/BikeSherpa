using Facet;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Model;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Model;

[Facet(typeof(Customer), exclude: [nameof(Customer.DomainEvents), nameof(Customer.Address)])]
public partial record CustomerCrud
{
     public required AddressCrud Address { get; init; }
}