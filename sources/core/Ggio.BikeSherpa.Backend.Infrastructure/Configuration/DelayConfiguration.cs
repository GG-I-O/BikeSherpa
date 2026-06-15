using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DelayConfiguration : IEntityTypeConfiguration<Delay>
{
     public void Configure(EntityTypeBuilder<Delay> builder)
     {
          builder.ToTable("Delays");
          builder.HasKey(d => d.Id);
          builder.Property(d => d.Id).ValueGeneratedNever();
          builder.Property(d => d.Price).IsRequired();
          builder.Property(d => d.Label).IsRequired();
          builder.HasData(new Delay
               {
                    Label = "Jour même",
                    Id = DelayRepository.StandardCost,
                    Price = 0
               },
               new Delay
               {
                    Label = "Prioritaire",
                    Id = DelayRepository.LastMinuteOrderExtraCost,
                    Price = 3.0
               },
               new Delay
               {
                    Label = "",
                    Id = DelayRepository.Earlyorderdiscount,
                    Price = -2.0
               });
     }
}
