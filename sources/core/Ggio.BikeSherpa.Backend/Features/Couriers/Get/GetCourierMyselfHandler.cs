using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Couriers.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Get;

public record GetCourierMyselfQuery(string Email) : IQuery<CourierCrud?>;

public class GetCourierMyselfHandler(
     IReadRepository<Courier> clientRepository
     ): IQueryHandler<GetCourierMyselfQuery, CourierCrud?>
{
     public async ValueTask<CourierCrud?> Handle(GetCourierMyselfQuery query, CancellationToken ct)
     {
          var entity = await clientRepository.FirstOrDefaultAsync(new CourierByEmailSpecification(query.Email), ct);
          return entity?.ToFacet<CourierCrud>();
     }
}
