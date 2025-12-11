using Facet;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Clients;

[Facet(typeof(Client), exclude:nameof(Client.DomainEvents))]
public partial record ClientCrud;