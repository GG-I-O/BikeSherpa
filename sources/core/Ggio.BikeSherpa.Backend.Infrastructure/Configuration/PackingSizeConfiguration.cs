using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class PackingSizeConfiguration : IEntityTypeConfiguration<PackingSize>
{
     public void Configure(EntityTypeBuilder<PackingSize> builder)
     {
          builder.ToTable("PackingSizes");
          builder.HasKey(s => s.Id);
          builder.Property(s => s.Id).ValueGeneratedOnAdd();
          builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
          builder.Property(s => s.MaxWeight).IsRequired();
          builder.Property(s => s.MaxPackageLength).IsRequired();
          builder.Property(s => s.TourPrice).IsRequired();
          builder.Property(s => s.Price).IsRequired();
          builder.HasData(
               new {
                    Name = "S",
                    MaxWeight = 3,
                    MaxPackageLength = 45,
                    TourPrice = 0,
                    Price = 3
               },
               new {
                    Name = "M",
                    MaxWeight = 10,
                    MaxPackageLength = 85,
                    TourPrice = 2,
                    Price = 5
               },
               new {
                    Id = 3,
                    Name = "L",
                    MaxWeight = 20,
                    MaxPackageLength = 105,
                    TourPrice = 4,
                    Price = 7
               },
               new {
                    Name = "Xl",
                    MaxWeight = 30,
                    MaxPackageLength = 115,
                    TourPrice = 6,
                    Price = 9
               },
               new {
                    Name = "Xxl",
                    MaxWeight = 40,
                    MaxPackageLength = 500,
                    TourPrice = 8,
                    Price = 11
               },
               new {
                    Name = "Xxxl",
                    MaxWeight = 50,
                    MaxPackageLength = 500,
                    TourPrice = 10,
                    Price = 13
               },
               new {
                    Name = "Xxxxl",
                    MaxWeight = 60,
                    MaxPackageLength = 500,
                    TourPrice = 12,
                    Price = 15
               }
          );
     }
}
