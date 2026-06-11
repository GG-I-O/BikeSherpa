using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;

public class CustomerByCodeAndEmailSpecification : SingleResultSpecification<Customer>
{
     
    public CustomerByCodeAndEmailSpecification(string code, string email)
    {
        Query.Where(c => c.Code == code.Trim() && c.Email == email.Trim());
    }
}
