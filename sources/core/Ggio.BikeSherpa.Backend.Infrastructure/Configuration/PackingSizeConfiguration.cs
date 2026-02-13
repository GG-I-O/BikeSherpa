using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class PackingSizeConfiguration : IEntityTypeConfiguration<PackingSizeEntity>
{
     public void Configure(EntityTypeBuilder<PackingSizeEntity> builder)
     {
          builder.HasKey(p => p.Id);

          builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
          builder.Property(p => p.MaxWeight).IsRequired();
          builder.Property(p => p.TourMaxLength).IsRequired();
          builder.Property(p => p.MaxLength).IsRequired();
          builder.Property(p => p.TourPrice).IsRequired();
          builder.Property(p => p.Price).IsRequired();

          builder.HasData(
               new PackingSizeEntity
               {
                    Id = 1,
                    Name = "S",
                    MaxWeight = 3,
                    TourMaxLength = 45,
                    MaxLength = 45,
                    TourPrice = 0,
                    Price = 3
               },
               new PackingSizeEntity
               {
                    Id = 2,
                    Name = "M",
                    MaxWeight = 10,
                    TourMaxLength = 55,
                    MaxLength = 85,
                    TourPrice = 2,
                    Price = 5
               },
               new PackingSizeEntity
               {
                    Id = 3,
                    Name = "L",
                    MaxWeight = 20,
                    TourMaxLength = 85,
                    MaxLength = 105,
                    TourPrice = 4,
                    Price = 7
               },
               new PackingSizeEntity
               {
                    Id = 4,
                    Name = "Xl",
                    MaxWeight = 30,
                    TourMaxLength = 105,
                    MaxLength = 115,
                    TourPrice = 6,
                    Price = 9
               },
               new PackingSizeEntity
               {
                    Id = 5,
                    Name = "Xxl",
                    MaxWeight = 40,
                    TourMaxLength = 105,
                    MaxLength = 115,
                    TourPrice = 8,
                    Price = 11
               }
          );


          builder.ToTable("PackingSizes");
     }
}
