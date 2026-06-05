using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Events;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.DomainEventHandlers;

public class CustomerCodeChangedEventHandler(IApplicationTransactionContext context, IReadRepository<Delivery> deliveryRepository) : TransactionalDomainEventHandlerBase<CustomerCodeChanged>(context)
{
     override protected async ValueTask HandleInternal(CustomerCodeChanged notification, CancellationToken cancellationToken)
     {
          var customerDeliveries = await deliveryRepository.ListAsync(new DeliveryByCustomerIdSpecification(notification.CustomerId), cancellationToken);
          if (customerDeliveries.Any())
          {
               foreach (var delivery in customerDeliveries)
               {
                    delivery.GenerateCode();
               }
          }
     }
}
