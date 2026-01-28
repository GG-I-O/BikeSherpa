using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.GetAll;

public class GetAllDeliveriesHandler(IReadRepository<Delivery> repository) : IQueryHandler<GetAllCoursesQuery, List<DeliveryCrud>>
{
     public async ValueTask<List<DeliveryCrud>> Handle(GetAllCoursesQuery query, CancellationToken cancellationToken)
     {
          var allCourse = (await repository.ListAsync(cancellationToken)).SelectFacets<DeliveryCrud>();
          return allCourse.ToList();
     }
}

public record GetAllCoursesQuery : IQuery<List<DeliveryCrud>>;
