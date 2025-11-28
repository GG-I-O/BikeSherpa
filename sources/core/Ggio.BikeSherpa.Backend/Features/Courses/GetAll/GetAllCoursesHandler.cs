using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses.GetAll;

public class GetAllCoursesHandler(IReadRepository<Course> repository) : IQueryHandler<GetAllCoursesQuery, List<CourseCrud>>
{
     public async ValueTask<List<CourseCrud>> Handle(GetAllCoursesQuery query, CancellationToken cancellationToken)
     {
          var allCourse = (await repository.ListAsync(cancellationToken)).SelectFacets<CourseCrud>();
          return allCourse.ToList();
     }
}

public record GetAllCoursesQuery : IQuery<List<CourseCrud>>;
