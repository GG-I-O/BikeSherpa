using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CourseAggregate.Specification;

public class CourseByIdSpecification : SingleResultSpecification<Course>
{
     public CourseByIdSpecification(Guid id)
     {
          Query.Where(x => x.Id == id);
     }
}
