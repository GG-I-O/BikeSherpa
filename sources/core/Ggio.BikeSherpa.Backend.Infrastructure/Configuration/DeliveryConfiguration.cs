using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
     public void Configure(EntityTypeBuilder<Delivery> builder)
     {
          builder.ToTable("Deliveries");
          builder.HasKey(d => d.Id);
          builder.Property(d => d.PricingStrategy).HasConversion<int>().IsRequired();
          builder.Property(d => d.Status).HasConversion<int>().IsRequired();
          builder.Property(d => d.Code).IsRequired();
          builder.Property(d => d.CustomerId).IsRequired();
          builder.Property(d => d.Urgency).IsRequired();
          builder.Property(d => d.TotalPrice).IsRequired();
          builder.Property(d => d.ReportId).IsRequired();
          builder.Property(d => d.Details).IsRequired();
          builder.Property(d => d.PackingSize).IsRequired();
          builder.Property(d => d.ContractDate).IsRequired();
          builder.Property(d => d.StartDate).IsRequired();
          builder.OwnsMany(d => d.Steps, steps =>
          {
               steps.WithOwner().HasForeignKey("DeliveryId");
               steps.HasKey("Id");
               steps.Property(s => s.StepType).HasConversion<int>().IsRequired();
               steps.Property(s => s.Order).IsRequired();
               steps.Property(s => s.Completed).IsRequired();
               steps.OwnsOne(s => s.StepAddress, address =>
               {
                    address.Property(a => a.Name).HasMaxLength(200).IsRequired();
                    address.Property(a => a.StreetInfo).HasMaxLength(200).IsRequired();
                    address.Property(a => a.Complement).HasMaxLength(200);
                    address.Property(a => a.Postcode).HasMaxLength(5).IsRequired();
                    address.Property(a => a.City).HasMaxLength(100).IsRequired();
               });
               steps.HasOne(s => s.StepZone).WithMany()
                    .HasForeignKey("StepZoneName")
                    .IsRequired();
               steps.Property(s => s.Distance).IsRequired();
               steps.Property(s => s.EstimatedDeliveryDate).IsRequired();
               steps.Property(s => s.RealDeliveryDate);
               steps.ToTable("DeliverySteps");
          });
     }
}
