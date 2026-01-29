using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
     public void Configure(EntityTypeBuilder<Courier> builder)
     {
          builder.HasKey(x => x.Id);
          builder.OwnsOne(x => x.Address);
     }
}
