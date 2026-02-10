using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          PricingStrategy pricingStrategy,
          DeliveryStatus status,
          string code,
          Guid customerId,
          Urgency urgency,
          double? totalPrice,
          Guid reportId,
          List<DeliveryStep> steps,
          string[] details,
          double weight,
          int length,
          Packing packing,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(IMediator mediator, IReadRepository<Customer> customerRepository) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategy pricingStrategy, DeliveryStatus status, string code, Guid customerId, Urgency urgency, double? totalPrice, Guid reportId, List<DeliveryStep> steps, string[] details, double weight, int length, Packing packing, DateTimeOffset contractDate, DateTimeOffset startDate)
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
               Code = code,
               PickupAddress = customer.Address,
               PickupZone = DeliveryZone.FromAddress(customer.Address.City),
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

          delivery.Steps = ChoosePricingStrategy(pricingStrategy).AddDeliverySteps(delivery, customer);
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
