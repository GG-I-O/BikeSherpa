using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
          PricingStrategy pricingStrategy,
          string code,
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

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(PricingStrategy pricingStrategy, string code, Guid customerId, string urgency, double? totalPrice, double? discount, string[] details, string packingSize, bool insulatedBox, DateTimeOffset contractDate, DateTimeOffset startDate)
     {
          var delivery = new Delivery()
          {
               PricingStrategy = pricingStrategy,
               Code = code,
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

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
