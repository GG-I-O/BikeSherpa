using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Get;

public record GetCourierQuery(Guid Id) : IQuery<CourierCrud?>;

public class GetCourierHandler(IReadRepository<Courier> clientRepository): IQueryHandler<GetCourierQuery, CourierCrud?>
{
     public async ValueTask<CourierCrud?> Handle(GetCourierQuery query, CancellationToken ct)
     {
          var entity = await clientRepository.FirstOrDefaultAsync(new CourierByIdSpecification(query.Id), ct);
          return entity?.ToFacet<CourierCrud>();
     }
}