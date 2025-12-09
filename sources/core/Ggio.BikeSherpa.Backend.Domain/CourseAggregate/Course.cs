using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.CourseAggregate;

public class Course : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public DateTimeOffset StartDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
}
