using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          PricingStrategy pricingStrategy,
          DeliveryStatus status,
          string code,
          double? totalPrice,
          Guid customerId,
          Guid reportId,
          List<DeliveryStep> steps,
          string[] details,
          double weight,
          int length,
          Packing packing,
          Urgency urgency,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(IMediator mediator, IReadRepository<Customer> customerRepository) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategy pricingStrategy, DeliveryStatus status, string code, double? totalPrice, Guid customerId, Guid reportId, List<DeliveryStep> steps, string[] details, double weight, int length, Packing packing, Urgency urgency, DateTimeOffset contractDate, DateTimeOffset startDate)
     {
          var customer = await customerRepository.SingleOrDefaultAsync(
               new CustomerByIdSpecification(customerId));

          if (customer == null)
          {
               throw new InvalidOperationException($"Impossible de trouver un client avec l'identifiant {customerId}");
          }

          var delivery = new Delivery
          {
               PricingStrategy = pricingStrategy,
               Status = status,
               PickupAddress = customer.Address,
               PickupZone = DeliveryZone.FromAddress(customer.Address.City),
               Code = code,
               CustomerId = customerId,
               Urgency = urgency,
               TotalPrice = totalPrice ?? 0,
               ReportId = reportId,
               Steps = steps,
               Details = details,
               Weight = weight,
               Length = length,
               Packing = new Packing(weight, length),
               ContractDate = contractDate,
               StartDate = startDate
          };
          
          delivery.TotalPrice = ChoosePricingStrategy(pricingStrategy).CalculatePrice(delivery);

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
     
     private IPricingStrategy ChoosePricingStrategy(PricingStrategy pricingStrategy)
     {
          if (pricingStrategy == PricingStrategy.CustomStrategy)
               return new CustomStrategy();
          
          if (pricingStrategy == PricingStrategy.SimpleDeliveryStrategy)
               return new SimpleDeliveryStrategy();
          
          if (pricingStrategy == PricingStrategy.TourDeliveryStrategy)
               return new TourDeliveryStrategy();

          throw new InvalidOperationException("Grille tarifaire inconnue.");
     }
}
