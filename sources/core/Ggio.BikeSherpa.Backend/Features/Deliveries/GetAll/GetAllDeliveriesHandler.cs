using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public record GetAllDeliveriesQuery(DateTimeOffset? LastSync): IQuery<List<Model.DeliveryCrud>>;

public class GetAllDeliveriesHandler(IReadRepository<Delivery> repository) : IQueryHandler<GetAllDeliveriesQuery, List<Model.DeliveryCrud>>
{
     public async ValueTask<List<Model.DeliveryCrud>> Handle(GetAllDeliveriesQuery query, CancellationToken cancellationToken)
     {
          var allDeliveries = query.LastSync is null ?
               (await repository.ListAsync(cancellationToken)).SelectFacets<Delivery, Model.DeliveryCrud>() :
               (await repository.ListAsync(new DeliveryByUpdatedAtSpecification(query.LastSync!.Value) ,cancellationToken)).SelectFacets<Delivery, Model.DeliveryCrud>();
          return allDeliveries.ToList();
     }
}
