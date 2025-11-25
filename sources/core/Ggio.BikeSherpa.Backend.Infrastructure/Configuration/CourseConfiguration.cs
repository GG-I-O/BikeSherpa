using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
     public void Configure(EntityTypeBuilder<Course> builder)
     {
          builder.HasKey(x => x.Id);
     }
}
