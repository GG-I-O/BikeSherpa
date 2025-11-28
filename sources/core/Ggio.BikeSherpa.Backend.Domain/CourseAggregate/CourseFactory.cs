using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CourseAggregate;

public interface ICourseFactory
{
     Task<Course> CreateCourseAsync(DateTimeOffset startDate) //on n'initialise que ce qui est required
          ;
}

public class CourseFactory(IMediator mediator) : FactoryBase(mediator), ICourseFactory
{
     public async Task<Course> CreateCourseAsync(DateTimeOffset startDate) //on n'initialise que ce qui est required
     {
          var newEntity= new Course
          {
               StartDate = startDate
          };
          await NotifyNewEntityAdded(newEntity);
          return newEntity;
     }
}
