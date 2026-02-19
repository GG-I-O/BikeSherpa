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
          double? totalPrice,
          double? discount,
          Guid reportId,
          string packingSize,
          bool insulatedBox,
          bool exactTime,
          DateTimeOffset contractDate,
          DateTimeOffset startDate
     );
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategyEnum pricingStrategyEnum, string code, Guid customerId, string urgency, double? totalPrice, double? discount, Guid reportId, string packingSize, bool insulatedBox, bool exactTime, DateTimeOffset contractDate, DateTimeOffset startDate)
     {
          var delivery = new Delivery
          {
               PricingStrategy = pricingStrategyEnum,
               Code = code,
               CustomerId = customerId,
               Urgency = urgency,
               TotalPrice = totalPrice,
               Discount = discount,
               ReportId = reportId,
               Steps = [],
               PackingSize = packingSize,
               InsulatedBox = insulatedBox,
               ExactTime = exactTime,
               ContractDate = contractDate,
               StartDate = startDate
          };

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
