using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          PricingStrategyEnum pricingStrategyEnum,
          DeliveryStatusEnum statusEnum,
          string code,
          Guid customerId,
          double? totalPrice,
          Guid reportId,
          string[] details,
          double totalWeight,
          int highestLength,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategyEnum pricingStrategyEnum, DeliveryStatusEnum statusEnum, string code, Guid customerId, double? totalPrice, Guid reportId, string[] details, double totalWeight, int highestLength, DateTimeOffset contractDate, DateTimeOffset startDate)
     {
          var delivery = new Delivery
          {
               PricingStrategyEnum = pricingStrategyEnum,
               StatusEnum = statusEnum,
               Code = code,
               CustomerId = customerId,
               Urgency = null,
               TotalPrice = totalPrice ?? 0,
               ReportId = reportId,
               Steps = [],
               Details = details,
               TotalWeight = totalWeight,
               HighestLength = highestLength,
               Size = null,
               ContractDate = contractDate,
               StartDate = startDate
          };

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
