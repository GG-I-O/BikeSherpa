using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.Get;

public record GetCourseQuery(Guid Id) : IQuery<DeliveryCrud?>;

public class GetDeliveryHandler(IReadRepository<Delivery> courseRepository) : IQueryHandler<GetCourseQuery, DeliveryCrud?>
{
     public async ValueTask<DeliveryCrud?> Handle(GetCourseQuery query, CancellationToken cancellationToken)
     {
          var entity = await courseRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(query.Id), cancellationToken);
          return entity?.ToFacet<DeliveryCrud>();
     }
}
