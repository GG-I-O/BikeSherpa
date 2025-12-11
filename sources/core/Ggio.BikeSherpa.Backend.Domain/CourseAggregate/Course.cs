using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.CourseAggregate;

public class Course : EntityBase<Guid>, IAggregateRoot
{
     public DateTimeOffset StartDate { get; set; }
}
