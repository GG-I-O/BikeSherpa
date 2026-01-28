using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;

public class CustomerByVatNumberSpecification : SingleResultSpecification<Customer>
{
     public CustomerByVatNumberSpecification(string vatNumber)
     {
          Query.Where(x => x.VatNumber == vatNumber);
     }
}