using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
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
          Packing packing,
          Urgency urgency,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(IMediator mediator, IReadRepository<Customer> customerRepository) : FactoryBase(mediator), IDeliveryFactory
{

     public async Task<Delivery> CreateDeliveryAsync(DeliveryStatus status, string code, Guid customerId, double totalPrice, Guid reportId, List<DeliveryStep> steps, string[] details, Packing packing, Urgency urgency, DateTimeOffset contractDate, DateTimeOffset startDate)
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
               PickupZone = DeliveryZone.FromAddress(customer.Address.City),
               Code = code,
               CustomerId = customerId,
               Urgency = urgency,
               TotalPrice = totalPrice,
               ReportId = reportId,
               Steps = steps,
               Details = details,
               Packing = packing,
               ContractDate = contractDate,
               StartDate = startDate
          };

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
