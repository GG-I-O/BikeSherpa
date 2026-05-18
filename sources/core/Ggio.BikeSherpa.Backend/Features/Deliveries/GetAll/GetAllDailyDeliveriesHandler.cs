using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;
using Facet.Extensions;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public record GetAllDailyDeliveriesQuery(
     string UserEmail,
     DateTimeOffset Date
) : IQuery<GetAllDailyDeliveriesResult>;

public class GetAllDailyDeliveriesHandler(
     IReadRepository<Courier> courierRepository,
     IReadRepository<Delivery> deliveryRepository
     ): IQueryHandler<GetAllDailyDeliveriesQuery, GetAllDailyDeliveriesResult>
{
     
     public async ValueTask<GetAllDailyDeliveriesResult> Handle(GetAllDailyDeliveriesQuery request, CancellationToken cancellationToken)
     {
          var courier = await courierRepository.FirstOrDefaultAsync(new CourierByEmailSpecification(request.UserEmail), cancellationToken);
          if (courier is null)
               return new GetAllDailyDeliveriesResult.CourierNotFound();
          
          var deliveries = (await deliveryRepository
               .ListAsync(new DeliveryStepByCourierAndDate(courier.Id, request.Date), cancellationToken))
               .SelectFacets<Delivery, DeliveryCrud>()
               .Select(delivery => delivery with
               {
                    Steps = delivery.Steps
                         .Where(s => s.Data.CourierId == courier.Id &&
                                     s.Data.EstimatedDeliveryDate.Date == request.Date.Date)
                         .OrderBy(s => s.Data.EstimatedDeliveryDate)
                         .ToList()
               })
               .ToList();
          
          return new GetAllDailyDeliveriesResult.Success(deliveries);
     }
}
