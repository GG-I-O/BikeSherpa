using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Deliveries;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public class GetAllDeliveriesHandler(IReadRepository<Delivery> repository) : IQueryHandler<GetAllDeliveriesQuery, List<DeliveryCrud>>
{
     public async ValueTask<List<DeliveryCrud>> Handle(GetAllDeliveriesQuery query, CancellationToken cancellationToken)
     {
          var allDeliveries = (await repository.ListAsync(cancellationToken)).SelectFacets<DeliveryCrud>();
          return allDeliveries.ToList();
     }
}

public record GetAllDeliveriesQuery : IQuery<List<DeliveryCrud>>;
