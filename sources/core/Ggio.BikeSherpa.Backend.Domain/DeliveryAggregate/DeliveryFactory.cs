using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          PricingStrategy pricingStrategy,
          Guid customerId,
          string urgency,
          double? totalPrice,
          double? discount,
          string[] details,
          string packingSize,
          bool insulatedBox,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(
     IMediator mediator,
     IReadRepository<Customer> customerRepository,
     IReadRepository<Delivery> deliveryRepository,
     IPricingStrategyService pricingStrategyService
) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategy pricingStrategy, Guid customerId, string urgency, double? totalPrice, double? discount, string[] details, string packingSize,
          bool insulatedBox, DateTimeOffset contractDate, DateTimeOffset startDate)
     {
          var delivery = new Delivery
          {
               Id = Guid.NewGuid(),
               PricingStrategy = pricingStrategy,
               Code = "",
               CustomerId = customerId,
               Urgency = urgency,
               TotalPrice = totalPrice,
               Discount = discount,
               Details = details,
               Steps = [],
               PackingSize = packingSize,
               InsulatedBox = insulatedBox,
               ContractDate = contractDate,
               StartDate = startDate
          };

          var customer = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(customerId));

          if (customer is not null)
          {
               delivery.GenerateReportId(customer);

               var deliveries = await deliveryRepository.ListAsync(new DeliveryByCodeLikeSpecification(customer.Code));
               delivery.GenerateCode(customer, deliveries.Count + 1);
          }

          delivery.TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(delivery);

          await NotifyNewAggregateRootAdded(delivery);

          return delivery;
     }
}
