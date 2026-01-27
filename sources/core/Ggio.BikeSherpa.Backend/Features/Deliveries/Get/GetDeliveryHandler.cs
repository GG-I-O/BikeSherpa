using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Get;

public record GetDeliveryQuery(Guid Id) : IQuery<DeliveryCrud?>;

public class GetDeliveryHandler(IReadRepository<Delivery> deliveryRepository) : IQueryHandler<GetDeliveryQuery, DeliveryCrud?>
{
     public async ValueTask<DeliveryCrud?> Handle(GetDeliveryQuery query, CancellationToken ct)
     {
          var entity = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(query.Id), ct);
          return entity?.ToFacet<DeliveryCrud>();
     }
}
