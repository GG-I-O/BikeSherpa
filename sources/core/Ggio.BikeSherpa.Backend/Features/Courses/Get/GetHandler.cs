using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;

public class GetCourseQuery(Guid Id) : IQuery<CourseDto>;

public class GetHandler : IQueryHandler<GetCourseQuery, CourseDto>
{
     public ValueTask<CourseDto> Handle(GetCourseQuery query, CancellationToken cancellationToken) => throw new NotImplementedException();
}
