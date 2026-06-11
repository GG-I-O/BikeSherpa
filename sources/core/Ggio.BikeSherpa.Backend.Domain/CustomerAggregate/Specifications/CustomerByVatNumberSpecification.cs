using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;

public class CustomerByVatNumberSpecification : SingleResultSpecification<Customer>
{
     public CustomerByVatNumberSpecification(string vatNumber)
     {
          Query.Where(x => x.VatNumber == vatNumber);
     }
}