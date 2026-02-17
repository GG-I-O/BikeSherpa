using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class PackingSizeConfiguration : IEntityTypeConfiguration<PackingSizeEntity>
{
     public void Configure(EntityTypeBuilder<PackingSizeEntity> builder)
     {
          builder.ToTable("PackingSizes");
          builder.HasKey(p => p.Id);
          builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
          builder.Property(p => p.MaxWeight).IsRequired();
          builder.Property(p => p.MaxPackageLength).IsRequired();
          builder.Property(p => p.TourPrice).IsRequired();
          builder.Property(p => p.Price).IsRequired();
          builder.HasData(
               new PackingSizeEntity
               {
                    Id = 1,
                    Name = "S",
                    MaxWeight = 3,
                    MaxPackageLength = 45,
                    TourPrice = 0,
                    Price = 3
               },
               new PackingSizeEntity
               {
                    Id = 2,
                    Name = "M",
                    MaxWeight = 10,
                    MaxPackageLength = 85,
                    TourPrice = 2,
                    Price = 5
               },
               new PackingSizeEntity
               {
                    Id = 3,
                    Name = "L",
                    MaxWeight = 20,
                    MaxPackageLength = 105,
                    TourPrice = 4,
                    Price = 7
               },
               new PackingSizeEntity
               {
                    Id = 4,
                    Name = "Xl",
                    MaxWeight = 30,
                    MaxPackageLength = 115,
                    TourPrice = 6,
                    Price = 9
               },
               new PackingSizeEntity
               {
                    Id = 5,
                    Name = "Xxl",
                    MaxWeight = 40,
                    MaxPackageLength = 500,
                    TourPrice = 8,
                    Price = 11
               },
               new PackingSizeEntity
               {
                    Id = 6,
                    Name = "Xxxl",
                    MaxWeight = 50,
                    MaxPackageLength = 500,
                    TourPrice = 10,
                    Price = 13
               },
               new PackingSizeEntity
               {
                    Id = 7,
                    Name = "Xxxxl",
                    MaxWeight = 60,
                    MaxPackageLength = 500,
                    TourPrice = 12,
                    Price = 15
               }
          );
     }
}
