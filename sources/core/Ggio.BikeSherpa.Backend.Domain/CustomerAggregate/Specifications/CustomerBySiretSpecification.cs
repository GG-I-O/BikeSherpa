using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;

public class CustomerBySiretSpecification : SingleResultSpecification<Customer>
{
     public CustomerBySiretSpecification(string siret)
     {
          Query.Where(x => x.Siret == siret);
     }
}