using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          PricingStrategyEnum pricingStrategyEnum,
          string code,
          Guid customerId,
          string urgency,
          Guid reportId,
          double totalWeight,
          int highestLength,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategyEnum pricingStrategyEnum, string code, Guid customerId, string urgency, Guid reportId, double totalWeight, int highestLength, DateTimeOffset contractDate, DateTimeOffset startDate)
     {
          var delivery = new Delivery
          {
               PricingStrategy = pricingStrategyEnum,
               Code = code,
               CustomerId = customerId,
               Urgency = urgency,
               ReportId = reportId,
               Steps = [],
               TotalWeight = totalWeight,
               HighestPackageLength = highestLength,
               ContractDate = contractDate,
               StartDate = startDate
          };

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
