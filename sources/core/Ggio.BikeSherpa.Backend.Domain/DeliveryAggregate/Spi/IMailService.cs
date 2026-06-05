using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

public interface IMailService
{
     Task SendSimpleDeliveryMailToCustomer(Delivery delivery, Customer customer);
     Task SendTourDeliveryMailToCustomer(Delivery delivery, Customer customer);
}
