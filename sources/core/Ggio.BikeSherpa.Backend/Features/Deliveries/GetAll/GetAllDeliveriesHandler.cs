using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public record GetAllDeliveriesQuery(DateTimeOffset? LastSync) : IQuery<List<Model.DeliveryCrud>>;

public class GetAllDeliveriesHandler(
     IReadRepository<Delivery> repository,
     IUrgencyRepository urgencyRepository
     ) : IQueryHandler<GetAllDeliveriesQuery, List<Model.DeliveryCrud>>
{
     public async ValueTask<List<Model.DeliveryCrud>> Handle(GetAllDeliveriesQuery query, CancellationToken cancellationToken)
     {
          var deliveries = query.LastSync is null
               ? await repository.ListAsync(cancellationToken)
               : await repository.ListAsync(new DeliveryByUpdatedAtSpecification(query.LastSync.Value), cancellationToken);

          var orderedDeliveries = deliveries
               .Select(delivery =>
               {
                    var deliveryCrud = delivery.ToFacet<Delivery, Model.DeliveryCrud>();

                    return deliveryCrud with
                    {
                         LimitDate = delivery.GetLimitDate(urgencyRepository),
                         Steps = deliveryCrud.Steps.OrderBy(s => s.Data.Order).ToList()
                    };
               })
               .ToList();

          return orderedDeliveries;
     }
}
