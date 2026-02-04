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
          string packing
     );
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), IDeliveryFactory
{

     public async Task<Delivery> CreateDeliveryAsync(DeliveryStatus status, string code, Guid customerId, double totalPrice, Guid reportId, List<DeliveryStep> steps, string[] details, string packing)
     {
          var delivery = new Delivery
          {
               Status = status,
               Code = code,
               CustomerId = customerId,
               TotalPrice = totalPrice,
               ReportId = reportId,
               Details = details,
               Packing = packing,
               Steps = steps
          };

          await NotifyNewEntityAdded(delivery);

          return delivery;
     }
}
