using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;
using Facet.Extensions;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public record GetAllUnassignedDeliveriesQuery(
     DateTimeOffset Date
) : IQuery<GetAllDailyDeliveriesResult>;

public class GetAllUnassignedDeliveriesHandler(
     IReadRepository<Delivery> deliveryRepository
     ): IQueryHandler<GetAllUnassignedDeliveriesQuery, GetAllDailyDeliveriesResult>
{
     
     public async ValueTask<GetAllDailyDeliveriesResult> Handle(GetAllUnassignedDeliveriesQuery request, CancellationToken cancellationToken)
     {
          var deliveries = (await deliveryRepository
               .ListAsync(new DeliveryStepByCourierAndDate(null, request.Date), cancellationToken))
               .SelectFacets<Delivery, DeliveryCrud>()
               .Select(delivery => delivery with
               {
                    Steps = delivery.Steps
                         .Where(s => s.Data.CourierId == null &&
                                     s.Data.EstimatedDeliveryDate.Date == request.Date.Date)
                         .OrderBy(s => s.Data.EstimatedDeliveryDate)
                         .ToList()
               })
               .ToList();
          
          return new GetAllDailyDeliveriesResult.Success(deliveries);
     }
}
