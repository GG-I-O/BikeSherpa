using Ggio.BikeSherpa.Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class ParameterConfiguration : IEntityTypeConfiguration<Parameter>
{
     public void Configure(EntityTypeBuilder<Parameter> builder)
     {
         builder.HasKey(p => p.Key);
         builder.ToTable("Parameters");
         builder.Property(p => p.Key).HasMaxLength(100).IsRequired();
         builder.Property(p => p.Value).IsRequired();
         builder.HasData(
              new
              {
                   Key = ParameterRepository.VatRateKey,
                   Value = "20"
              },
              new
              {
                   Key = ParameterRepository.LastHourToOrder,
                   Value = "15"
              },
              new
              {
                   Key = ParameterRepository.WorkStartDate,
                   Value = new DateTimeOffset(1,1,1,8,0,0,0,new TimeSpan(0)).ToString("yyyy-MM-ddTHH:mm:ss")
              },
              new
              {
                   Key = ParameterRepository.WorkEndDate,
                   Value = new DateTimeOffset(1,1,1,19,0,0,0,new TimeSpan(0)).ToString("yyyy-MM-ddTHH:mm:ss")
              },
              new
              {
                   Key = ParameterRepository.SimpleDeliveryMailTemplateKey,
                   Value = "Your delivery is ready for pickup at {pickupLocation} on {pickupDate}. {deliverycode} { pickupaddress} { destinationaddress} {PickupDate} {LoadingSlot} "
              },
              new
              {
                   Key = ParameterRepository.SimpleDeliveryMailSubjectKey,
                   Value = "Delivery Ready for Pickup"
              },
              new
              {
                   Key = ParameterRepository.TourDeliveryMailTemplateKey,
                   Value = "Your delivery is ready for pickup at {pickupLocation} on {pickupDate}. {deliverycode} { pickupaddress} { destinationaddress} {PickupDate} {LoadingSlot} "
              },
              new
              {
                   Key = ParameterRepository.TourDeliveryMailSubjectKey,
                   Value = "Tour delivery Ready for Pickup"
              }
         );
     }
}
