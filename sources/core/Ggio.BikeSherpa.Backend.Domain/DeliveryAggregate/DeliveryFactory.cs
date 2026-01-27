using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
               string code,
               string customerId,
               double totalPrice,
               string reportId,
               string[] stepIds,
               string[] details,
               string packing
          );
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{

     public async Task<Delivery> CreateDeliveryAsync(string code, string customerId, double totalPrice, string reportId, string[] stepIds, string[] details, string packing)
     {
          var delivery = new Delivery
          {
               Code = code,
               CustomerId = customerId,
               TotalPrice = totalPrice,
               ReportId = reportId,
               StepIds = stepIds,
               Details = details,
               Packing = packing
          };
          
          await NotifyNewEntityAdded(delivery);
          
          return delivery;
     }
}
