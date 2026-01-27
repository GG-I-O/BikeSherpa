using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(
               string code,
               Customer customer,
               double totalPrice,
               string reportId,
               string[] steps,
               string[] details,
               string packing)
          ;
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{

     public async Task<Delivery> CreateDeliveryAsync(string code, Customer customer, double totalPrice, string reportId, string[] steps, string[] details, string packing)
     {
          var delivery = new Delivery
          {
               Code = code,
               Customer = customer,
               TotalPrice = totalPrice,
               ReportId = reportId,
               Steps = steps,
               Details = details,
               Packing = packing
          };
          
          await NotifyNewEntityAdded(delivery);
          
          return delivery;
     }
}
