using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Customer>
{
     public void Configure(EntityTypeBuilder<Customer> builder)
     {
          builder.HasKey(x => x.Id);
     }
}
