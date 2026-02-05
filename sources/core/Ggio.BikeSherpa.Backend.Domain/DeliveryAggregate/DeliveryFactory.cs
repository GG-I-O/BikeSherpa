using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          DeliveryStatus status,
          string code,
          Guid customerId,
          double totalPrice,
          Guid reportId,
          List<DeliveryStep> steps,
          string[] details,
          string packing
     );
}

public class DeliveryFactory(IMediator mediator, IReadRepository<Customer> customerRepository) : FactoryBase(mediator), IDeliveryFactory
{

     public async Task<Delivery> CreateDeliveryAsync(DeliveryStatus status, string code, Guid customerId, double totalPrice, Guid reportId, List<DeliveryStep> steps, string[] details, string packing)
     {
          var customer = await customerRepository.SingleOrDefaultAsync(
               new CustomerByIdSpecification(customerId));

          if (customer == null)
          {
               throw new InvalidOperationException($"Impossible de trouver un client avec l'identifiant {customerId}");
          }

          var delivery = new Delivery
          {
               Status = status,
               PickupAddress = customer.Address,
               Code = code,
               CustomerId = customerId,
               TotalPrice = totalPrice,
               ReportId = reportId,
               Details = details,
               Packing = packing,
               Steps = steps
          };

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
