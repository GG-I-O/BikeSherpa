using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers;

namespace BackendTests.Features.Customers;

public static class CustomerTestHelper
{
     public static Customer CreateCustomer(
          Guid id,
          string name,
          string code,
          string? siret,
          string email,
          string phoneNumber,
          Address address
     )
     {
          return new Customer
          {
               Id = id,
               Name = name,
               Code = code,
               Siret = siret,
               Email = email,
               PhoneNumber = phoneNumber,
               Address = address
          };
     }
     
     public static CustomerCrud CreateCustomerCrud(
          Guid id,
          string name,
          string code,
          string? siret,
          string email,
          string phoneNumber,
          Address address
     )
     {
          return new CustomerCrud
          {
               Id = id,
               Name = name,
               Code = code,
               Siret = siret,
               Email = email,
               PhoneNumber = phoneNumber,
               Address = address
          };
     }
}
