using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.ClientAggregate.Specification;

public class ClientByIdSpecification : SingleResultSpecification<Client>
{
     public ClientByIdSpecification(Guid id)
     {
          Query.Where(x => x.Id == id);
     }
}
