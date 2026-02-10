using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.GetAll;

public record GetAllCouriersQuery(DateTimeOffset? LastSync) : IQuery<List<Model.CourierCrud>>;

public class GetAllCouriersHandler(IReadRepository<Courier> repository) : IQueryHandler<GetAllCouriersQuery, List<Model.CourierCrud>>
{
     public async ValueTask<List<Model.CourierCrud>> Handle(GetAllCouriersQuery query, CancellationToken cancellationToken)
     {
          var allCouriers = query.LastSync is null ?
               (await repository.ListAsync(cancellationToken)).SelectFacets<Courier, Model.CourierCrud>() :
               (await repository.ListAsync(new CourierByUpdatedAtSpecification(query.LastSync!.Value), cancellationToken)).SelectFacets<Courier, Model.CourierCrud>();
          return allCouriers.ToList();
     }
}
