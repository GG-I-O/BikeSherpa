using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class CourseConfiguration : IEntityTypeConfiguration<Delivery>
{
     public void Configure(EntityTypeBuilder<Delivery> builder)
     {
          builder.HasKey(x => x.Id);
     }
}
