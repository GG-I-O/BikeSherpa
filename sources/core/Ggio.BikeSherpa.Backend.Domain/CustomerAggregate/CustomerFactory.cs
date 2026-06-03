using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public record CustomerFactoryParameters(
     string Name,
     string Code,
     string? Siret,
     string? VatNumber,
     string Email,
     string PhoneNumber,
     Address Address,
     DeliveryType? DefaultDeliveryType);

public interface ICustomerFactory
{
     Task<Customer> CreateCustomerAsync(CustomerFactoryParameters parameters);
}

public class CustomerFactory(IMediator mediator) : FactoryBase(mediator), ICustomerFactory
{
     public async Task<Customer> CreateCustomerAsync(CustomerFactoryParameters parameters)
     {
          var customer = new Customer
          {
               Name = parameters.Name,
               Code = parameters.Code,
               Siret = parameters.Siret,
               VatNumber = parameters.VatNumber,
               Email = parameters.Email,
               PhoneNumber = parameters.PhoneNumber,
               Address = parameters.Address,
               DefaultDeliveryType = parameters.DefaultDeliveryType
          };

          await NotifyNewAggregateRootAdded(customer);
          return customer;
     }
}
