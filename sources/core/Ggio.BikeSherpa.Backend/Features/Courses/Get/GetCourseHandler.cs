using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;

public record GetCourseQuery(Guid Id) : IQuery<CourseCrud?>;

public class GetCourseHandler(IReadRepository<Course> courseRepository) : IQueryHandler<GetCourseQuery, CourseCrud?>
{
     public async ValueTask<CourseCrud?> Handle(GetCourseQuery query, CancellationToken cancellationToken)
     {
          var entity = await courseRepository.FirstOrDefaultAsync(new CourseByIdSpecification(query.Id), cancellationToken);
          return entity?.ToFacet<CourseCrud>();
     }
}
