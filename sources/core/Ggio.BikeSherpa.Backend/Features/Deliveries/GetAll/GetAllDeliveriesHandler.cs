using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public record GetAllDeliveriesQuery(DateTimeOffset? LastSync) : IQuery<List<DeliveryCrud>>;

public class GetAllDeliveriesHandler(
     IReadRepository<Delivery> repository
) : IQueryHandler<GetAllDeliveriesQuery, List<DeliveryCrud>>
{
     public async ValueTask<List<DeliveryCrud>> Handle(GetAllDeliveriesQuery query, CancellationToken cancellationToken)
     {
          var allDeliveries = query.LastSync is null
               ? (await repository.ListAsync(cancellationToken)).SelectFacets<Delivery, DeliveryCrud>()
               : (await repository.ListAsync(new DeliveryByUpdatedAtSpecification(query.LastSync!.Value), cancellationToken)).SelectFacets<Delivery, DeliveryCrud>();

          var orderedDeliveries = allDeliveries
               .Select(delivery => delivery with
               {
                    Steps = delivery.Steps.OrderBy(s => s.Data.Order).ToList()
               })
               .ToList();

          return orderedDeliveries;
     }
}
